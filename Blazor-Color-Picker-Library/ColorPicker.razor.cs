using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorColorPicker;

public partial class ColorPicker
{
    private static IReadOnlyList<string> DefaultColors => PaletteColorMath.DefaultColors;

    [Inject] private IColorPickerService? ColorPickerService { get; set; } = default!;

    private ColorPickerParameters Parameters { get; set; } = new();

    private TaskCompletionSource<PickerResult>? Tcs { get; set; }

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

    // ---- Intensity / Hue sliders (default Material palette only) ----

    private const int IntensityMagnet = 4;
    private const int HueMagnet = 7;

    /// <summary>Reference cell for the slider tracks when nothing is selected (Material Blue 500).</summary>
    private static readonly int DefaultReferenceCell = PaletteColorMath.IndexOf("#2196F3");

    private bool IsDefaultPalette { get; set; } = true;

    private bool IsAdjustMode => IsDefaultPalette && (Parameters.ShowIntensitySlider || Parameters.ShowHueSlider);

    private int _intensity = PaletteChoice.NeutralIntensity;
    private int _hue = PaletteChoice.NeutralHue;
    /// <summary>Index of the checked cell, kept while the sliders change its color.</summary>
    private int? _selected;
    private bool _keyboardInput;
    private string _intensityTrack = string.Empty;
    private string _hueTrack = string.Empty;

    private string AdjustCssClass => Parameters.ShowIntensitySlider
        ? "color-picker-adjust color-picker-adjust-with-intensity"
        : "color-picker-adjust";

    protected override void OnInitialized()
    {
        if (ColorPickerService is ColorPickerService service)
        {
            service.Register(ShowColorPicker);
        }
    }

    private Task<PickerResult> ShowColorPicker(ColorPickerParameters parameters)
    {
        Parameters = parameters;
        Tcs = new TaskCompletionSource<PickerResult>();
        IsVisible = true;

        IsDefaultPalette = parameters.MyColorPallet is not { Length: > 0 };

        // Use custom palette if provided, otherwise use default colors (zero allocation)
        colors = IsDefaultPalette
            ? DefaultColors
            : parameters.MyColorPallet;

        HighlightedColor = null;
        _selected = null;
        _intensity = PaletteChoice.NeutralIntensity;
        _hue = PaletteChoice.NeutralHue;
        _keyboardInput = false;

        if (IsAdjustMode)
        {
            // Restore the stored state, otherwise deduce it from the hex
            var choice = parameters.PaletteChoice ?? PaletteChoice.FromHex(parameters.ColorSelected);
            var index = PaletteColorMath.IndexOf(choice?.BaseHex);
            if (choice is not null && index >= 0)
            {
                _selected = index;
                _intensity = choice.Intensity;
                _hue = choice.Hue;
            }
            RefreshAdjustedPalette();
        }
        else
        {
            // Determine which color to highlight
            HighlightedColor = DetermineHighlightedColor(parameters.ColorSelected, colors, parameters.FindClosestIfNotFound);
        }

        StateHasChanged();

        return Tcs.Task;
    }

    private void RefreshAdjustedPalette()
    {
        colors = PaletteColorMath.ComputeGrid(_intensity, _hue);

        var reference = _selected ?? DefaultReferenceCell;
        _intensityTrack = PaletteColorMath.IntensityTrackGradient(reference, _hue);
        _hueTrack = PaletteColorMath.HueTrackGradient(reference, _intensity);
    }

    private void SetIntensity(int value)
    {
        value = Math.Clamp(value, PaletteChoice.MinIntensity, PaletteChoice.MaxIntensity);
        _intensity = _keyboardInput ? value : PaletteColorMath.Snap(value, PaletteChoice.NeutralIntensity, IntensityMagnet);
        OnSliderChanged();
    }

    private void SetHue(int value)
    {
        value = Math.Clamp(value, PaletteChoice.MinHue, PaletteChoice.MaxHue);
        _hue = _keyboardInput ? value : PaletteColorMath.Snap(value, PaletteChoice.NeutralHue, HueMagnet);
        OnSliderChanged();
    }

    private void OnSliderChanged()
    {
        RefreshAdjustedPalette();

        if (_selected is { } index)
        {
            Parameters.OnPaletteChoiceChanged?.Invoke(PaletteChoice.Create(index, _intensity, _hue));
        }
    }

    private void SliderKeyDown() => _keyboardInput = true;

    private void SliderPointerDown() => _keyboardInput = false;

    protected void KeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Escape")
        {
            HandleCancel();
        }
    }

    private void ColorClick(int index)
    {
        var color = colors[index];
        PaletteChoice? choice = null;

        if (IsDefaultPalette && index < PaletteColorMath.CellCount)
        {
            choice = IsAdjustMode
                ? PaletteChoice.Create(index, _intensity, _hue)
                : PaletteChoice.Create(index, PaletteChoice.NeutralIntensity, PaletteChoice.NeutralHue);
            color = choice.Hex;
            Parameters.OnPaletteChoiceChanged?.Invoke(choice);
        }

        Tcs?.TrySetResult(new PickerResult(color, choice));
        Close();
    }

    private void HandleCancel()
    {
        Tcs?.TrySetResult(new PickerResult(Parameters.ColorSelected, Parameters.PaletteChoice));
        Close();
    }

    private void Close()
    {
        IsVisible = false;
        HighlightedColor = null;
        StateHasChanged();
    }

    private bool IsColorHighlighted(int index, string item)
    {
        // Sliders: the check is remembered by indices and follows the adjusted color
        if (IsAdjustMode)
            return _selected == index;

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
