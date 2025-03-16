using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorColorPicker.Tests;

public class ColorPickerTests : TestContext
{
    private const int TEST_TIMEOUT = 3000;

    public ColorPickerTests()
    {
        Services.AddColorPicker();
        ColorPickerService = Services.GetRequiredService<IColorPickerService>();

        // Vérifie que le service est bien injecté
        Assert.NotNull(ColorPickerService);
    }

    public IColorPickerService ColorPickerService { get; }

    [Fact]
    public async Task ColorPicker_Open()
    {
        using var cts = new CancellationTokenSource(TEST_TIMEOUT);

        // Act
        var parameters = new ColorPickerParameters
        {
            ColorSelected = "#F1F7E9",
        };
        var dialogTask = ColorPickerService.ShowColorPicker(parameters);

        // Simuler un délai pour vérifier si ça fonctionne bien
        await Task.Delay(100, cts.Token);

        // Assert
        Assert.NotNull(dialogTask);
    }
}