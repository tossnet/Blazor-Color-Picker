using System.Globalization;
using System.Text;

namespace BlazorColorPicker;

/// <summary>
/// HSL computations for the Material palette and its Intensity / Hue sliders.
/// Every cell always starts from its original Material color, so results never drift.
/// </summary>
internal static class PaletteColorMath
{
    internal static readonly IReadOnlyList<string> DefaultColors =
    [
        // Red
        "#FFE4E9", "#FFCDD2", "#EE9A9A", "#E57373", "#EE534F",
        "#F44236", "#E53935", "#C9342D", "#C32C28", "#B61C1C",
        // Rose
        "#FFD2E7", "#F9BBD0", "#F48FB1", "#F06292", "#EC407A",
        "#EA1E63", "#D81A60", "#C2175B", "#AD1457", "#890E4F",
        // Mauve
        "#F8D5FF", "#E1BEE8", "#CF93D9", "#B968C7", "#AA47BC",
        "#9C28B1", "#8E24AA", "#7A1FA2", "#6A1B9A", "#4A148C",
        // Violet
        "#E7DBFF", "#D0C4E8", "#B39DDB", "#9675CE", "#7E57C2",
        "#673BB7", "#5D35B0", "#512DA7", "#45289F", "#301B92",
        // Bleu foncé
        "#DCE1FF", "#C5CAE8", "#9EA8DB", "#7986CC", "#5C6BC0",
        "#3F51B5", "#3949AB", "#303E9F", "#283593", "#1A237E",
        // Bleu
        "#D2F5FF", "#BBDEFA", "#90CAF8", "#64B5F6", "#42A5F6",
        "#2196F3", "#1D89E4", "#1976D3", "#1564C0", "#0E47A1",
        // Cyan
        "#CAFCFF", "#B3E5FC", "#81D5FA", "#4FC2F8", "#28B6F6",
        "#03A9F5", "#039BE6", "#0288D1", "#0277BD", "#00579C",
        // Bleu-Vert
        "#C9FFFF", "#B2EBF2", "#80DEEA", "#4DD0E2", "#25C6DA",
        "#00BCD5", "#00ACC2", "#0098A6", "#00828F", "#016064",
        // Bleu-vert foncé
        "#C9F6F3", "#B2DFDC", "#80CBC4", "#4CB6AC", "#26A59A",
        "#009788", "#00887A", "#00796A", "#00695B", "#004C3F",
        // Vert
        "#DFFDE1", "#C8E6CA", "#A5D6A7", "#80C783", "#66BB6A",
        "#4CB050", "#43A047", "#398E3D", "#2F7D32", "#1C5E20",
        // Green-Yellow
        "#F4FFDF", "#DDEDC8", "#C5E1A6", "#AED582", "#9CCC66",
        "#8BC24A", "#7DB343", "#689F39", "#548B2E", "#33691E",
        // Green-Yellow-Light
        "#FFFFD9", "#F0F4C2", "#E6EE9B", "#DDE776", "#D4E056",
        "#CDDC39", "#C0CA33", "#B0B42B", "#9E9E24", "#817716",
        // Yellow
        "#FFFFDA", "#FFFAC3", "#FFF59C", "#FFF176", "#FFEE58",
        "#FFEB3C", "#FDD734", "#FAC02E", "#F9A825", "#F47F16",
        // Yellow-Orange
        "#FFFFC9", "#FFECB2", "#FFE083", "#FFD54F", "#FFC928",
        "#FEC107", "#FFB200", "#FF9F00", "#FF8E01", "#FF6F00",
        // Orange
        "#FFF7C9", "#FFE0B2", "#FFCC80", "#FFB64D", "#FFA827",
        "#FF9700", "#FB8C00", "#F67C01", "#EF6C00", "#E65100",
        // Orange Dark
        "#FFE3D2", "#FFCCBB", "#FFAB91", "#FF8A66", "#FF7143",
        "#FE5722", "#F5511E", "#E64A19", "#D74315", "#BF360C",
        // Marron
        "#EEE3DF", "#D7CCC8", "#BCABA4", "#A0887E", "#8C6E63",
        "#7B5347", "#6D4D42", "#5D4038", "#4D342F", "#3E2622",
        // Grey
        "#FFFFFF", "#F5F5F5", "#EEEEEE", "#E0E0E0", "#BDBDBD",
        "#9E9E9E", "#757575", "#616161", "#424242", "#212121",
        // Bleu gris
        "#E5F0F4", "#CED9DD", "#B0BFC6", "#90A4AD", "#798F9A",
        "#607D8B", "#546F7A", "#465A65", "#36474F", "#273238"
    ];

    internal const int CellCount = 190; // 19 columns x 10 rows

    /// <summary>Plain-color landing at each end of a slider track (half the thumb size).</summary>
    private const int TrackPadPx = 12;

    /// <summary>Base HSL of the 190 Material cells, computed once.</summary>
    internal static readonly Hsl[] BaseHsl = DefaultColors
        .Select(hex => TryParseHex(hex, out var rgb) ? RgbToHsl(rgb) : default)
        .ToArray();

    internal readonly record struct Hsl(double H, double S, double L);

    private static readonly Dictionary<string, int> IndexByHex = DefaultColors
        .Select((hex, i) => (hex, i))
        .ToDictionary(x => x.hex, x => x.i, StringComparer.OrdinalIgnoreCase);

    /// <summary>Index of a Material base color in the grid, or -1 if it is not part of the palette.</summary>
    internal static int IndexOf(string? baseHex)
        => baseHex is not null && IndexByHex.TryGetValue(baseHex.Trim(), out var i) ? i : -1;

    internal static Hsl Adjust(Hsl baseColor, double intensity, double hue)
    {
        var h = baseColor.S == 0
            ? baseColor.H
            : (baseColor.H + hue + 360) % 360;

        var t = (intensity - 100) / 100;
        var s = Clamp01(baseColor.S * (t < 0 ? 1 + 0.9 * t : 1 + t));
        var l = Clamp01(baseColor.L - 0.15 * t);

        return new Hsl(h, s, l);
    }

    internal static string AdjustedHex(int index, double intensity, double hue)
        => ToHex(HslToRgb(Adjust(BaseHsl[index], intensity, hue)));

    /// <summary>Computes the 190 adjusted colors in one pass.</summary>
    internal static string[] ComputeGrid(int intensity, int hue)
    {
        var result = new string[CellCount];
        for (var i = 0; i < CellCount; i++)
        {
            result[i] = ToHex(HslToRgb(Adjust(BaseHsl[i], intensity, hue)));
        }
        return result;
    }

    /// <summary>Vertical track, top to bottom: intensity 200 (t = +1) down to 0 (t = -1), at the current hue.</summary>
    internal static string IntensityTrackGradient(int index, int hue)
    {
        const int samples = 21;
        var colors = new string[samples];
        for (var k = 0; k < samples; k++)
        {
            colors[k] = AdjustedHex(index, 200 - 200.0 * k / (samples - 1), hue);
        }
        return BuildGradient("to bottom", colors);
    }

    /// <summary>Horizontal track, left to right: hue -180 to +180, at the current intensity.</summary>
    internal static string HueTrackGradient(int index, int intensity)
    {
        const int samples = 25;
        var colors = new string[samples];
        for (var k = 0; k < samples; k++)
        {
            colors[k] = AdjustedHex(index, intensity, -180 + 360.0 * k / (samples - 1));
        }
        return BuildGradient("to right", colors);
    }

    private static string BuildGradient(string direction, string[] colors)
    {
        var sb = new StringBuilder("linear-gradient(");
        sb.Append(direction);
        sb.Append(", ").Append(colors[0]).Append(" 0");
        for (var k = 0; k < colors.Length; k++)
        {
            var f = (double)k / (colors.Length - 1);
            sb.Append(", ").Append(colors[k])
              .Append(" calc(").Append(TrackPadPx).Append("px + (100% - ").Append(TrackPadPx * 2).Append("px) * ")
              .Append(f.ToString("0.#####", CultureInfo.InvariantCulture)).Append(')');
        }
        sb.Append(", ").Append(colors[^1]).Append(" 100%)");
        return sb.ToString();
    }

    /// <summary>Magnet: brings the value back to neutral when within the radius.</summary>
    internal static int Snap(int value, int neutral, int radius)
        => Math.Abs(value - neutral) <= radius ? neutral : value;

    internal static Hsl RgbToHsl((int R, int G, int B) rgb)
    {
        var r = rgb.R / 255.0;
        var g = rgb.G / 255.0;
        var b = rgb.B / 255.0;

        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var l = (max + min) / 2;
        var d = max - min;

        if (d == 0)
            return new Hsl(0, 0, l);

        var s = l > 0.5 ? d / (2 - max - min) : d / (max + min);

        double h;
        if (max == r)
            h = (g - b) / d + (g < b ? 6 : 0);
        else if (max == g)
            h = (b - r) / d + 2;
        else
            h = (r - g) / d + 4;

        return new Hsl(h * 60, s, l);
    }

    internal static (int R, int G, int B) HslToRgb(Hsl hsl)
    {
        if (hsl.S == 0)
        {
            var v = ToByte(hsl.L);
            return (v, v, v);
        }

        var q = hsl.L < 0.5 ? hsl.L * (1 + hsl.S) : hsl.L + hsl.S - hsl.L * hsl.S;
        var p = 2 * hsl.L - q;
        var h = hsl.H / 360;

        return (ToByte(HueToRgb(p, q, h + 1.0 / 3)),
                ToByte(HueToRgb(p, q, h)),
                ToByte(HueToRgb(p, q, h - 1.0 / 3)));
    }

    private static double HueToRgb(double p, double q, double t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1.0 / 6) return p + (q - p) * 6 * t;
        if (t < 1.0 / 2) return q;
        if (t < 2.0 / 3) return p + (q - p) * (2.0 / 3 - t) * 6;
        return p;
    }

    private static int ToByte(double v)
        => (int)Math.Clamp(Math.Round(v * 255, MidpointRounding.AwayFromZero), 0, 255);

    private static double Clamp01(double v) => Math.Clamp(v, 0, 1);

    internal static string ToHex((int R, int G, int B) c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

    internal static double Distance((int R, int G, int B) a, (int R, int G, int B) b)
    {
        var dr = a.R - b.R;
        var dg = a.G - b.G;
        var db = a.B - b.B;
        return Math.Sqrt(dr * dr + dg * dg + db * db);
    }

    internal static bool TryParseHex(string? hex, out (int R, int G, int B) color)
    {
        color = default;

        if (string.IsNullOrWhiteSpace(hex))
            return false;

        var span = hex.AsSpan().Trim().TrimStart('#');
        if (span.Length != 6)
            return false;

        if (int.TryParse(span[..2], NumberStyles.HexNumber, null, out var r) &&
            int.TryParse(span[2..4], NumberStyles.HexNumber, null, out var g) &&
            int.TryParse(span[4..6], NumberStyles.HexNumber, null, out var b))
        {
            color = (r, g, b);
            return true;
        }

        return false;
    }
}
