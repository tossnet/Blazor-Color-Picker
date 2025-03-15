namespace BlazorColorPicker;

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
    private event Func<ColorPickerParameters, Task<string>>? OnShowColorPicker;

    /// <summary>
    /// Registers a handler for the color picker event.
    /// </summary>
    /// <param name="handler">The handler to register.</param>
    internal void Register(Func<ColorPickerParameters, Task<string>> handler)
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
            return await OnShowColorPicker.Invoke(parameters);
        }

        return null;
    }
}
