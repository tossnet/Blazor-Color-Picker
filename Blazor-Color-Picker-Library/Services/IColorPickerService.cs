namespace BlazorColorPicker;

public interface IColorPickerService
{
    /// <summary>Opens the color picker and returns the selected color (or ColorSelected if cancelled).</summary>
    Task<string> ShowColorPicker(ColorPickerParameters parameters);

    /// <summary>
    /// Opens the Material palette and returns its full state (cell, sliders and color).
    /// Returns <see cref="ColorPickerParameters.PaletteChoice"/> unchanged if cancelled,
    /// and null when a custom palette (MyColorPallet) is used.
    /// </summary>
    Task<PaletteChoice?> ShowPaletteChoice(ColorPickerParameters parameters);
}
