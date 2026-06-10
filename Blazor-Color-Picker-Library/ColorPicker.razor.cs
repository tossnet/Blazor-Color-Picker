using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorColorPicker;

public partial class ColorPicker
{
    private static readonly IReadOnlyList<string> DefaultColors =
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

    [Inject] private IColorPickerService? ColorPickerService { get; set; } = default!;

    private ColorPickerParameters Parameters { get; set; } = new();

    private TaskCompletionSource<string>? Tcs { get; set; }

    private bool IsVisible { get; set; }

    private string CustomStyle => !string.IsNullOrWhiteSpace(Parameters.OverwriteBackgroundColor)
        ? $"background-color:{Parameters.OverwriteBackgroundColor};"
        : string.Empty;

    private string PaletteStyle
    {
        get
        {
            const int cellSize = 32;
            const int maxRowsSmall = 5;  // Pour petites palettes (≤50 couleurs)
            const int maxRowsLarge = 10; // Pour grandes palettes (>50 couleurs)
            const int maxColumns = 20;

            var totalColors = colors.Count;

            // Calculate rows and columns based on color count
            int rows, columns;

            if (totalColors == 0)
            {
                rows = 1;
                columns = 1;
            }
            else if (totalColors <= maxRowsSmall)
            {
                // Very small palette: single row
                rows = 1;
                columns = totalColors;
            }
            else if (totalColors <= 50)
            {
                // Small to medium palette: max 5 rows
                rows = maxRowsSmall;
                columns = (int)Math.Ceiling((double)totalColors / rows);
            }
            else
            {
                // Large palette: max 10 rows
                rows = maxRowsLarge;
                columns = (int)Math.Ceiling((double)totalColors / rows);

                // Cap columns at maximum
                if (columns > maxColumns)
                {
                    columns = maxColumns;
                    rows = (int)Math.Ceiling((double)totalColors / columns);
                }
            }

            // Calculate optimal dimensions
            var width = columns * cellSize;
            var height = rows * cellSize;

            // Parse custom dimensions from Parameters.Style if present
            var customWidth = ParseDimension(Parameters.Style, "width");
            var customHeight = ParseDimension(Parameters.Style, "height");

            // For mobile: swap custom dimensions or computed dimensions
            var mobileWidth = customHeight ?? $"{height}px";
            var mobileHeight = customWidth ?? $"{width}px";

            return $"--color-picker-rows: {rows}; --color-picker-columns: {columns}; " +
                   $"--color-picker-computed-width: {width}px; --color-picker-computed-height: {height}px; " +
                   $"--color-picker-mobile-width: {mobileWidth}; --color-picker-mobile-height: {mobileHeight}; " +
                   $"width: {customWidth ?? $"{width}px"}; height: {customHeight ?? $"{height}px"};";
        }
    }

    private static string? ParseDimension(string? style, string property)
    {
        if (string.IsNullOrWhiteSpace(style))
            return null;

        // Simple regex-free parsing: find "width:" or "height:"
        var index = style.IndexOf($"{property}:", StringComparison.OrdinalIgnoreCase);
        if (index == -1)
            return null;

        var start = index + property.Length + 1; // Skip "property:"
        var end = style.IndexOf(';', start);
        if (end == -1)
            end = style.Length;

        return style.Substring(start, end - start).Trim();
    }

    private string CssClass => IsVisible ? "color-picker-show" : "color-picker-hide";

    private string ModalStyle => Parameters.ZIndex.HasValue
        ? $"--color-picker-z-index: {Parameters.ZIndex.Value};"
        : string.Empty;

    private IReadOnlyList<string> colors = DefaultColors;

    private string? HighlightedColor { get; set; }

    protected ElementReference Element;

    protected override void OnInitialized()
    {
        if (ColorPickerService is ColorPickerService service)
        {
            service.Register(ShowColorPicker);
        }
    }

    private Task<string> ShowColorPicker(ColorPickerParameters parameters)
    {
        Parameters = parameters;
        Tcs = new TaskCompletionSource<string>();
        IsVisible = true;

        // Use custom palette if provided, otherwise use default colors (zero allocation)
        colors = parameters.MyColorPallet is { Length: > 0 }
            ? parameters.MyColorPallet
            : DefaultColors;

        // Determine which color to highlight
        HighlightedColor = DetermineHighlightedColor(parameters.ColorSelected, colors, parameters.FindClosestIfNotFound);

        StateHasChanged();

        return Tcs.Task;
    }

    protected void KeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Escape")
        {
            HandleCancel();
        }
    }

    private void ColorClick(string color)
    {
        Tcs?.SetResult(color);
        Close();
    }

    private void HandleCancel()
    {
        Tcs?.SetResult(Parameters.ColorSelected);
        Close();
    }

    private void Close()
    {
        IsVisible = false;
        HighlightedColor = null;
        StateHasChanged();
    }

    private bool IsColorHighlighted(string item)
    {
        // If we found a closest color match, use it
        if (HighlightedColor is not null)
            return string.Equals(HighlightedColor, item, StringComparison.OrdinalIgnoreCase);

        // Default behavior: case-insensitive match with selected color
        return string.Equals(Parameters.ColorSelected, item, StringComparison.OrdinalIgnoreCase);
    }

    private static string? DetermineHighlightedColor(string colorSelected, IReadOnlyList<string> palette, bool findClosest)
    {
        // Only compute closest color when explicitly requested
        if (!findClosest || string.IsNullOrWhiteSpace(colorSelected))
            return null;

        // If color already exists in palette, no need to find closest
        if (palette.Any(c => c.Equals(colorSelected, StringComparison.OrdinalIgnoreCase)))
            return null;

        // Find closest color
        return TryParseHexColor(colorSelected, out var target)
            ? FindClosestColor(target, palette)
            : null;
    }

    private static string? FindClosestColor((int R, int G, int B) target, IReadOnlyList<string> palette)
    {
        string? closest = null;
        var minDistance = double.MaxValue;

        foreach (var hex in palette)
        {
            if (!TryParseHexColor(hex, out var color))
                continue;

            var distance = ColorDistance(target, color);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = hex;
            }
        }

        return closest;
    }

    private static double ColorDistance((int R, int G, int B) c1, (int R, int G, int B) c2)
    {
        // Weighted Euclidean distance for better perceptual accuracy
        var rMean = (c1.R + c2.R) / 2.0;
        var dR = c1.R - c2.R;
        var dG = c1.G - c2.G;
        var dB = c1.B - c2.B;

        return Math.Sqrt((2 + rMean / 256) * dR * dR + 4 * dG * dG + (2 + (255 - rMean) / 256) * dB * dB);
    }

    private static bool TryParseHexColor(string hex, out (int R, int G, int B) color)
    {
        color = default;

        if (string.IsNullOrWhiteSpace(hex))
            return false;

        var span = hex.AsSpan().TrimStart('#');
        if (span.Length != 6)
            return false;

        if (int.TryParse(span[..2], System.Globalization.NumberStyles.HexNumber, null, out var r) &&
            int.TryParse(span[2..4], System.Globalization.NumberStyles.HexNumber, null, out var g) &&
            int.TryParse(span[4..6], System.Globalization.NumberStyles.HexNumber, null, out var b))
        {
            color = (r, g, b);
            return true;
        }

        return false;
    }
}
