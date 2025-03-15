namespace BlazorColorPicker;

public interface IColorPickerService
{
    Task<string> ShowColorPicker(ColorPickerParameters parameters);
}
