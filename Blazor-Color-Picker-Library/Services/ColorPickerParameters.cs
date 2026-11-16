namespace BlazorColorPicker;

public record ColorPickerParameters
{
    public string Title { get; init; } = string.Empty;
    public string ColorSelected { get; init; } = string.Empty;
    public string[] MyColorPallet { get; init; }
    public string OverwriteBackgroundColor { get; init; }
    public string? Style { get; init; } = null;

    /// <summary>
    /// When true and ColorSelected is not in the palette, the closest color will be highlighted.
    /// </summary>
    public bool FindClosestIfNotFound { get; init; } = false;

    /// <summary>
    /// Overrides the default z-index (9999). Useful when the host app has a parent with a higher z-index.
    /// Can also be set globally via the CSS custom property --color-picker-z-index.
    /// </summary>
    public int? ZIndex { get; init; } = null;

    /// <summary>
    /// Shows the vertical "Intensity" slider on the right of the Material palette (default palette only).
    /// </summary>
    public bool ShowIntensitySlider { get; init; } = false;

    /// <summary>
    /// Shows the horizontal "Hue" slider under the Material palette (default palette only).
    /// </summary>
    public bool ShowHueSlider { get; init; } = false;

    /// <summary>
    /// Palette state to restore (selected cell and slider positions). When null, the state is
    /// deduced from <see cref="ColorSelected"/> with <see cref="BlazorColorPicker.PaletteChoice.FromHex"/>.
    /// </summary>
    public PaletteChoice? PaletteChoice { get; init; }

    /// <summary>
    /// Called each time the palette state changes (slider moved while a cell is selected, or cell clicked).
    /// </summary>
    public Action<PaletteChoice>? OnPaletteChoiceChanged { get; init; }
}
