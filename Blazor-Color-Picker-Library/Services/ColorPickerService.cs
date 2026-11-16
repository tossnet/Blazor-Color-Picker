namespace BlazorColorPicker;

internal sealed record PickerResult(string Hex, PaletteChoice? Choice);

public class ColorPickerService : IColorPickerService
{
    private readonly IServiceProvider _serviceProvider;

    public ColorPickerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Event triggered to show the color picker.
    /// </summary>
    private event Func<ColorPickerParameters, Task<PickerResult>>? OnShowColorPicker;

    /// <summary>
    /// Registers a handler for the color picker event.
    /// </summary>
    /// <param name="handler">The handler to register.</param>
    internal void Register(Func<ColorPickerParameters, Task<PickerResult>> handler)
    {
        OnShowColorPicker += handler;
    }

    /// <summary>
    /// Shows the color picker with the specified parameters.
    /// </summary>
    /// <param name="parameters">The parameters for the color picker.</param>
    /// <returns>The selected color as a string, or null if no color was selected.</returns>
    public async Task<string> ShowColorPicker(ColorPickerParameters parameters)
    {
        if (OnShowColorPicker is not null)
        {
            var result = await OnShowColorPicker.Invoke(parameters);
            return result.Hex;
        }

        return null;
    }

    /// <inheritdoc />
    public async Task<PaletteChoice?> ShowPaletteChoice(ColorPickerParameters parameters)
    {
        if (OnShowColorPicker is not null)
        {
            var result = await OnShowColorPicker.Invoke(parameters);
            return result.Choice;
        }

        return parameters.PaletteChoice;
    }
}
