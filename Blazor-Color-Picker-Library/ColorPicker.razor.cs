using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorColorPicker;

public partial class ColorPicker
{
    [Inject] private IColorPickerService? ColorPickerService { get; set; } = default!;

    /// </summary>
    private ColorPickerParameters Parameters { get; set; } = new ColorPickerParameters();

    /// </summary>
    private TaskCompletionSource<string>? Tcs { get; set; }

    /// </summary>
    private bool IsVisible { get; set; }

    /// </summary>
    private string CustomStyle => !string.IsNullOrWhiteSpace(Parameters.OverwriteBackgroundColor) ? $"background-color:{Parameters.OverwriteBackgroundColor};" : string.Empty;
    
    /// </summary>
    private string CssClass => IsVisible ? "color-picker-show" : "color-picker-hide";


    /// </summary>
    private List<string> colorsBase = new();

    /// </summary>
    private List<string> colors = new();

    /// </summary>
    protected ElementReference Element;

    protected override void OnInitialized()
    {
        if (ColorPickerService is ColorPickerService service)
        {
            service.Register(ShowColorPicker);
        }
    }

    /// </summary>
    protected override void OnParametersSet()
    {
		#region PROVIDE COLORS LIST
			
		   //The default colors
		   // Red
		   colorsBase.Add("#FFE4E9");
           colorsBase.Add("#FFCDD2");
           colorsBase.Add("#EE9A9A");
           colorsBase.Add("#E57373");
           colorsBase.Add("#EE534F");
           colorsBase.Add("#F44236");
           colorsBase.Add("#E53935");
           colorsBase.Add("#C9342D");
           colorsBase.Add("#C32C28");
           colorsBase.Add("#B61C1C");
           // Rose
           colorsBase.Add("#FFD2E7");
           colorsBase.Add("#F9BBD0");
           colorsBase.Add("#F48FB1");
           colorsBase.Add("#F06292");
           colorsBase.Add("#EC407A");
           colorsBase.Add("#EA1E63");
           colorsBase.Add("#D81A60");
           colorsBase.Add("#C2175B");
           colorsBase.Add("#AD1457");
           colorsBase.Add("#890E4F");
           //Mauve
           colorsBase.Add("#F8D5FF");
           colorsBase.Add("#E1BEE8");
           colorsBase.Add("#CF93D9");
           colorsBase.Add("#B968C7");
           colorsBase.Add("#AA47BC");
           colorsBase.Add("#9C28B1");
           colorsBase.Add("#8E24AA");
           colorsBase.Add("#7A1FA2");
           colorsBase.Add("#6A1B9A");
           colorsBase.Add("#4A148C");
           //Violet
           colorsBase.Add("#E7DBFF");
           colorsBase.Add("#D0C4E8");
           colorsBase.Add("#B39DDB");
           colorsBase.Add("#9675CE");
           colorsBase.Add("#7E57C2");
           colorsBase.Add("#673BB7");
           colorsBase.Add("#5D35B0");
           colorsBase.Add("#512DA7");
           colorsBase.Add("#45289F");
           colorsBase.Add("#301B92");
           //Bleu foncé
           colorsBase.Add("#DCE1FF");
           colorsBase.Add("#C5CAE8");
           colorsBase.Add("#9EA8DB");
           colorsBase.Add("#7986CC");
           colorsBase.Add("#5C6BC0");
           colorsBase.Add("#3F51B5");
           colorsBase.Add("#3949AB");
           colorsBase.Add("#303E9F");
           colorsBase.Add("#283593");
           colorsBase.Add("#1A237E");
           //Bleu
           colorsBase.Add("#D2F5FF");
           colorsBase.Add("#BBDEFA");
           colorsBase.Add("#90CAF8");
           colorsBase.Add("#64B5F6");
           colorsBase.Add("#42A5F6");
           colorsBase.Add("#2196F3");
           colorsBase.Add("#1D89E4");
           colorsBase.Add("#1976D3");
           colorsBase.Add("#1564C0");
           colorsBase.Add("#0E47A1");
           //Cyan
           colorsBase.Add("#CAFCFF");
           colorsBase.Add("#B3E5FC");
           colorsBase.Add("#81D5FA");
           colorsBase.Add("#4FC2F8");
           colorsBase.Add("#28B6F6");
           colorsBase.Add("#03A9F5");
           colorsBase.Add("#039BE6");
           colorsBase.Add("#0288D1");
           colorsBase.Add("#0277BD");
           colorsBase.Add("#00579C");
           // Bleu-Vert
           colorsBase.Add("#C9FFFF");
           colorsBase.Add("#B2EBF2");
           colorsBase.Add("#80DEEA");
           colorsBase.Add("#4DD0E2");
           colorsBase.Add("#25C6DA");
           colorsBase.Add("#00BCD5");
           colorsBase.Add("#00ACC2");
           colorsBase.Add("#0098A6");
           colorsBase.Add("#00828F");
           colorsBase.Add("#016064");
           //Bleu-vert foncé
           colorsBase.Add("#C9F6F3");
           colorsBase.Add("#B2DFDC");
           colorsBase.Add("#80CBC4");
           colorsBase.Add("#4CB6AC");
           colorsBase.Add("#26A59A");
           colorsBase.Add("#009788");
           colorsBase.Add("#00887A");
           colorsBase.Add("#00796A");
           colorsBase.Add("#00695B");
           colorsBase.Add("#004C3F");
           //Vert
           colorsBase.Add("#DFFDE1");
           colorsBase.Add("#C8E6CA");
           colorsBase.Add("#A5D6A7");
           colorsBase.Add("#80C783");
           colorsBase.Add("#66BB6A");
           colorsBase.Add("#4CB050");
           colorsBase.Add("#43A047");
           colorsBase.Add("#398E3D");
           colorsBase.Add("#2F7D32");
           colorsBase.Add("#1C5E20");
           //Green-Yellow
           colorsBase.Add("#F4FFDF");
           colorsBase.Add("#DDEDC8");
           colorsBase.Add("#C5E1A6");
           colorsBase.Add("#AED582");
           colorsBase.Add("#9CCC66");
           colorsBase.Add("#8BC24A");
           colorsBase.Add("#7DB343");
           colorsBase.Add("#689F39");
           colorsBase.Add("#548B2E");
           colorsBase.Add("#33691E");
           //Green-Yellow-Light
           colorsBase.Add("#FFFFD9");
           colorsBase.Add("#F0F4C2");
           colorsBase.Add("#E6EE9B");
           colorsBase.Add("#DDE776");
           colorsBase.Add("#D4E056");
           colorsBase.Add("#CDDC39");
           colorsBase.Add("#C0CA33");
           colorsBase.Add("#B0B42B");
           colorsBase.Add("#9E9E24");
           colorsBase.Add("#817716");
           //Yellow
           colorsBase.Add("#FFFFDA");
           colorsBase.Add("#FFFAC3");
           colorsBase.Add("#FFF59C");
           colorsBase.Add("#FFF176");
           colorsBase.Add("#FFEE58");
           colorsBase.Add("#FFEB3C");
           colorsBase.Add("#FDD734");
           colorsBase.Add("#FAC02E");
           colorsBase.Add("#F9A825");
           colorsBase.Add("#F47F16");
           //Yellow-Orange
           colorsBase.Add("#FFFFC9");
           colorsBase.Add("#FFECB2");
           colorsBase.Add("#FFE083");
           colorsBase.Add("#FFD54F");
           colorsBase.Add("#FFC928");
           colorsBase.Add("#FEC107");
           colorsBase.Add("#FFB200");
           colorsBase.Add("#FF9F00");
           colorsBase.Add("#FF8E01");
           colorsBase.Add("#FF6F00");
           // Orange
           colorsBase.Add("#FFF7C9");
           colorsBase.Add("#FFE0B2");
           colorsBase.Add("#FFCC80");
           colorsBase.Add("#FFB64D");
           colorsBase.Add("#FFA827");
           colorsBase.Add("#FF9700");
           colorsBase.Add("#FB8C00");
           colorsBase.Add("#F67C01");
           colorsBase.Add("#EF6C00");
           colorsBase.Add("#E65100");
           //Orange Dark
           colorsBase.Add("#FFE3D2");
           colorsBase.Add("#FFCCBB");
           colorsBase.Add("#FFAB91");
           colorsBase.Add("#FF8A66");
           colorsBase.Add("#FF7143");
           colorsBase.Add("#FE5722");
           colorsBase.Add("#F5511E");
           colorsBase.Add("#E64A19");
           colorsBase.Add("#D74315");
           colorsBase.Add("#BF360C");
           //Marron
           colorsBase.Add("#EEE3DF");
           colorsBase.Add("#D7CCC8");
           colorsBase.Add("#BCABA4");
           colorsBase.Add("#A0887E");
           colorsBase.Add("#8C6E63");
           colorsBase.Add("#7B5347");
           colorsBase.Add("#6D4D42");
           colorsBase.Add("#5D4038");
           colorsBase.Add("#4D342F");
           colorsBase.Add("#3E2622");
           //Grey
           colorsBase.Add("#FFFFFF");
           colorsBase.Add("#F5F5F5");
           colorsBase.Add("#EEEEEE");
           colorsBase.Add("#E0E0E0");
           colorsBase.Add("#BDBDBD");
           colorsBase.Add("#9E9E9E");
           colorsBase.Add("#757575");
           colorsBase.Add("#616161");
           colorsBase.Add("#424242");
           colorsBase.Add("#212121");
           //Bleu gris
           colorsBase.Add("#E5F0F4");
           colorsBase.Add("#CED9DD");
           colorsBase.Add("#B0BFC6");
           colorsBase.Add("#90A4AD");
           colorsBase.Add("#798F9A");
           colorsBase.Add("#607D8B");
           colorsBase.Add("#546F7A");
           colorsBase.Add("#465A65");
           colorsBase.Add("#36474F");
           colorsBase.Add("#273238");
			
        #endregion
    }

    /// </summary>
    private Task<string> ShowColorPicker(ColorPickerParameters parameters)
    {
        Parameters = parameters;
        Tcs = new TaskCompletionSource<string>();
        IsVisible = true;

        colors.Clear();
        //My own color pallet
        if (Parameters.MyColorPallet is not null && Parameters.MyColorPallet.Any())
        {
            colors.AddRange(Parameters.MyColorPallet);
        }
        else
        {
            colors = new List<string>(colorsBase);
        }

        StateHasChanged();

        return Tcs.Task;
    }

    /// </summary>
    protected void KeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Escape")
        {
            HandleCancel();
        }
    }

    /// </summary>
    private void ColorClick(string color)
    {
        Tcs?.SetResult(color);
        Close();
    }

    /// </summary>
    private void HandleCancel()
    {
        Tcs?.SetResult(Parameters.ColorSelected);
        Close();
    }

    /// </summary>
    private void Close()
    {
        IsVisible = false;
        StateHasChanged();
    }
}