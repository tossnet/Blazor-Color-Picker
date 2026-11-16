using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorColorPicker;

/// <summary>
/// State of the Material palette: the chosen cell (identified by its original Material color)
/// and the slider positions, plus the resulting color.
/// Store it alongside your color setting to reopen the palette exactly where it was.
/// </summary>
/// <param name="BaseHex">Original Material color of the chosen cell, e.g. "#2196F3".</param>
/// <param name="Intensity">Intensity slider 0..200 (100 = neutral).</param>
/// <param name="Hue">Hue slider -180..180 (0 = neutral).</param>
/// <param name="Hex">Resulting color, "#RRGGBB".</param>
public sealed record PaletteChoice(string BaseHex, int Intensity, int Hue, string Hex)
{
    public const int MinIntensity = 0;
    public const int MaxIntensity = 200;
    public const int NeutralIntensity = 100;
    public const int MinHue = -180;
    public const int MaxHue = 180;
    public const int NeutralHue = 0;

    /// <summary>Maximum RGB distance accepted by <see cref="FromHex"/>.</summary>
    public const double FromHexThreshold = 12.0;

    /// <summary>
    /// Creates a choice from a Material palette color and computes the resulting color.
    /// Slider values are clamped to their ranges.
    /// </summary>
    /// <param name="baseHex">A color of the default Material palette, e.g. "#2196F3".</param>
    /// <exception cref="ArgumentException"><paramref name="baseHex"/> is not a color of the palette.</exception>
    public static PaletteChoice Create(string baseHex, int intensity = NeutralIntensity, int hue = NeutralHue)
    {
        var index = PaletteColorMath.IndexOf(baseHex);
        if (index < 0)
            throw new ArgumentException($"'{baseHex}' is not a color of the Material palette.", nameof(baseHex));

        return Create(index, intensity, hue);
    }

    internal static PaletteChoice Create(int index, int intensity, int hue)
    {
        intensity = Math.Clamp(intensity, MinIntensity, MaxIntensity);
        hue = Math.Clamp(hue, MinHue, MaxHue);

        return new PaletteChoice(
            PaletteColorMath.DefaultColors[index].ToUpperInvariant(),
            intensity,
            hue,
            PaletteColorMath.AdjustedHex(index, intensity, hue));
    }

    /// <summary>
    /// Finds the palette state that best reproduces <paramref name="hex"/>.
    /// Returns null when no state comes within <see cref="FromHexThreshold"/>.
    /// </summary>
    public static PaletteChoice? FromHex(string? hex)
    {
        if (!PaletteColorMath.TryParseHex(hex, out var target))
            return null;

        var targetHsl = PaletteColorMath.RgbToHsl(target);

        PaletteChoice? best = null;
        var bestDistance = double.MaxValue;
        var bestOffset = int.MaxValue;

        for (var index = 0; index < PaletteColorMath.CellCount; index++)
        {
            var baseHsl = PaletteColorMath.BaseHsl[index];

            var dh = 0.0;
            if (baseHsl.S > 0)
            {
                dh = ((targetHsl.H - baseHsl.H) % 360 + 360) % 360;
                if (dh > 180) dh -= 360;
            }

            var t = Math.Clamp((baseHsl.L - targetHsl.L) / 0.15, -1, 1);
            var intensity = (int)Math.Round(100 + t * 100, MidpointRounding.AwayFromZero);
            var hue = (int)Math.Round(dh, MidpointRounding.AwayFromZero);

            var candidate = Create(index, intensity, hue);
            PaletteColorMath.TryParseHex(candidate.Hex, out var rgb);
            var distance = PaletteColorMath.Distance(rgb, target);
            var offset = Math.Abs(candidate.Intensity - NeutralIntensity) + Math.Abs(candidate.Hue);

            if (distance < bestDistance - 1e-9 ||
                (Math.Abs(distance - bestDistance) <= 1e-9 && offset < bestOffset))
            {
                best = candidate;
                bestDistance = distance;
                bestOffset = offset;
            }
        }

        return bestDistance <= FromHexThreshold ? best : null;
    }

    /// <summary>Serializes the state as JSON.</summary>
    public string ToJson() => JsonSerializer.Serialize(this, PaletteChoiceJsonContext.Default.PaletteChoice);

    /// <summary>Reads a state written by <see cref="ToJson"/>. Returns null if missing, invalid or out of range.</summary>
    public static PaletteChoice? FromJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            var value = JsonSerializer.Deserialize(json, PaletteChoiceJsonContext.Default.PaletteChoice);
            var index = PaletteColorMath.IndexOf(value?.BaseHex);
            if (value is null ||
                index < 0 ||
                value.Intensity is < MinIntensity or > MaxIntensity ||
                value.Hue is < MinHue or > MaxHue)
            {
                return null;
            }

            // The computation is deterministic: recompute rather than trust the stored hex.
            return Create(index, value.Intensity, value.Hue);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>Uses the stored state when present, otherwise falls back to <see cref="FromHex"/>.</summary>
    public static PaletteChoice? Resolve(string? json, string? hex)
        => FromJson(json) ?? FromHex(hex);
}

[JsonSerializable(typeof(PaletteChoice))]
internal partial class PaletteChoiceJsonContext : JsonSerializerContext
{
}
