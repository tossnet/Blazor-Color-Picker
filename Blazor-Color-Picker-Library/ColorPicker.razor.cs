namespace BlazorColorPicker;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

public partial class ColorPicker
{
    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public string MyColor { get; set; }

    [Parameter]
    public bool IsOpened { get; set; }

    [Parameter]
    public string[] MyColorPallet { get; set; }

    [Parameter]
    public string OverwriteBackgroundColor { get; set; }

    [Parameter]
    public EventCallback<string> Closed { get; set; }

    [Parameter]
    public string? Style { get; set; } = "";

    private string CustomStyle => !string.IsNullOrWhiteSpace(OverwriteBackgroundColor) ? $"background-color:{OverwriteBackgroundColor};" : "";
    private string CssClass => IsOpened ? "color-picker-show" : "color-picker-hide";
    private List<string> colors = new();
    protected ElementReference myPalette; // set by the @ref attribute
		
    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        await myPalette.FocusAsync();
    }

    protected override void OnParametersSet()
    {
			colors.Clear();
			//My own color pallet
			if (MyColorPallet is not null && MyColorPallet.Any())
			{
				colors.AddRange(MyColorPallet);
				return;
			}
			
			#region PROVIDE COLORS LIST
			
			//The default colors
			// Red
			colors.Add("#FFE4E9");
           colors.Add("#FFCDD2");
           colors.Add("#EE9A9A");
           colors.Add("#E57373");
           colors.Add("#EE534F");
           colors.Add("#F44236");
           colors.Add("#E53935");
           colors.Add("#C9342D");
           colors.Add("#C32C28");
           colors.Add("#B61C1C");
           // Rose
           colors.Add("#FFD2E7");
           colors.Add("#F9BBD0");
           colors.Add("#F48FB1");
           colors.Add("#F06292");
           colors.Add("#EC407A");
           colors.Add("#EA1E63");
           colors.Add("#D81A60");
           colors.Add("#C2175B");
           colors.Add("#AD1457");
           colors.Add("#890E4F");
           //Mauve
           colors.Add("#F8D5FF");
           colors.Add("#E1BEE8");
           colors.Add("#CF93D9");
           colors.Add("#B968C7");
           colors.Add("#AA47BC");
           colors.Add("#9C28B1");
           colors.Add("#8E24AA");
           colors.Add("#7A1FA2");
           colors.Add("#6A1B9A");
           colors.Add("#4A148C");
           //Violet
           colors.Add("#E7DBFF");
           colors.Add("#D0C4E8");
           colors.Add("#B39DDB");
           colors.Add("#9675CE");
           colors.Add("#7E57C2");
           colors.Add("#673BB7");
           colors.Add("#5D35B0");
           colors.Add("#512DA7");
           colors.Add("#45289F");
           colors.Add("#301B92");
           //Bleu fonc�
           colors.Add("#DCE1FF");
           colors.Add("#C5CAE8");
           colors.Add("#9EA8DB");
           colors.Add("#7986CC");
           colors.Add("#5C6BC0");
           colors.Add("#3F51B5");
           colors.Add("#3949AB");
           colors.Add("#303E9F");
           colors.Add("#283593");
           colors.Add("#1A237E");
           //Bleu
           colors.Add("#D2F5FF");
           colors.Add("#BBDEFA");
           colors.Add("#90CAF8");
           colors.Add("#64B5F6");
           colors.Add("#42A5F6");
           colors.Add("#2196F3");
           colors.Add("#1D89E4");
           colors.Add("#1976D3");
           colors.Add("#1564C0");
           colors.Add("#0E47A1");
           //Cyan
           colors.Add("#CAFCFF");
           colors.Add("#B3E5FC");
           colors.Add("#81D5FA");
           colors.Add("#4FC2F8");
           colors.Add("#28B6F6");
           colors.Add("#03A9F5");
           colors.Add("#039BE6");
           colors.Add("#0288D1");
           colors.Add("#0277BD");
           colors.Add("#00579C");
           // Bleu-Vert
           colors.Add("#C9FFFF");
           colors.Add("#B2EBF2");
           colors.Add("#80DEEA");
           colors.Add("#4DD0E2");
           colors.Add("#25C6DA");
           colors.Add("#00BCD5");
           colors.Add("#00ACC2");
           colors.Add("#0098A6");
           colors.Add("#00828F");
           colors.Add("#016064");
           //Bleu-vert fonc�
           colors.Add("#C9F6F3");
           colors.Add("#B2DFDC");
           colors.Add("#80CBC4");
           colors.Add("#4CB6AC");
           colors.Add("#26A59A");
           colors.Add("#009788");
           colors.Add("#00887A");
           colors.Add("#00796A");
           colors.Add("#00695B");
           colors.Add("#004C3F");
           //Vert
           colors.Add("#DFFDE1");
           colors.Add("#C8E6CA");
           colors.Add("#A5D6A7");
           colors.Add("#80C783");
           colors.Add("#66BB6A");
           colors.Add("#4CB050");
           colors.Add("#43A047");
           colors.Add("#398E3D");
           colors.Add("#2F7D32");
           colors.Add("#1C5E20");
           //Green-Yellow
           colors.Add("#F4FFDF");
           colors.Add("#DDEDC8");
           colors.Add("#C5E1A6");
           colors.Add("#AED582");
           colors.Add("#9CCC66");
           colors.Add("#8BC24A");
           colors.Add("#7DB343");
           colors.Add("#689F39");
           colors.Add("#548B2E");
           colors.Add("#33691E");
           //Green-Yellow-Light
           colors.Add("#FFFFD9");
           colors.Add("#F0F4C2");
           colors.Add("#E6EE9B");
           colors.Add("#DDE776");
           colors.Add("#D4E056");
           colors.Add("#CDDC39");
           colors.Add("#C0CA33");
           colors.Add("#B0B42B");
           colors.Add("#9E9E24");
           colors.Add("#817716");
           //Yellow
           colors.Add("#FFFFDA");
           colors.Add("#FFFAC3");
           colors.Add("#FFF59C");
           colors.Add("#FFF176");
           colors.Add("#FFEE58");
           colors.Add("#FFEB3C");
           colors.Add("#FDD734");
           colors.Add("#FAC02E");
           colors.Add("#F9A825");
           colors.Add("#F47F16");
           //Yellow-Orange
           colors.Add("#FFFFC9");
           colors.Add("#FFECB2");
           colors.Add("#FFE083");
           colors.Add("#FFD54F");
           colors.Add("#FFC928");
           colors.Add("#FEC107");
           colors.Add("#FFB200");
           colors.Add("#FF9F00");
           colors.Add("#FF8E01");
           colors.Add("#FF6F00");
           // Orange
           colors.Add("#FFF7C9");
           colors.Add("#FFE0B2");
           colors.Add("#FFCC80");
           colors.Add("#FFB64D");
           colors.Add("#FFA827");
           colors.Add("#FF9700");
           colors.Add("#FB8C00");
           colors.Add("#F67C01");
           colors.Add("#EF6C00");
           colors.Add("#E65100");
           //Orange Dark
           colors.Add("#FFE3D2");
           colors.Add("#FFCCBB");
           colors.Add("#FFAB91");
           colors.Add("#FF8A66");
           colors.Add("#FF7143");
           colors.Add("#FE5722");
           colors.Add("#F5511E");
           colors.Add("#E64A19");
           colors.Add("#D74315");
           colors.Add("#BF360C");
           //Marron
           colors.Add("#EEE3DF");
           colors.Add("#D7CCC8");
           colors.Add("#BCABA4");
           colors.Add("#A0887E");
           colors.Add("#8C6E63");
           colors.Add("#7B5347");
           colors.Add("#6D4D42");
           colors.Add("#5D4038");
           colors.Add("#4D342F");
           colors.Add("#3E2622");
           //Grey
           colors.Add("#FFFFFF");
           colors.Add("#F5F5F5");
           colors.Add("#EEEEEE");
           colors.Add("#E0E0E0");
           colors.Add("#BDBDBD");
           colors.Add("#9E9E9E");
           colors.Add("#757575");
           colors.Add("#616161");
           colors.Add("#424242");
           colors.Add("#212121");
           //Bleu gris
           colors.Add("#E5F0F4");
           colors.Add("#CED9DD");
           colors.Add("#B0BFC6");
           colors.Add("#90A4AD");
           colors.Add("#798F9A");
           colors.Add("#607D8B");
           colors.Add("#546F7A");
           colors.Add("#465A65");
           colors.Add("#36474F");
           colors.Add("#273238");
			
        #endregion
    }

    protected void KeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Escape")
        {
            StateHasChanged();
            Closed.InvokeAsync(MyColor);
        }
    }

    private void ColorClick(string color)
    {
        MyColor = color;
        StateHasChanged();
        Closed.InvokeAsync(MyColor);
    }

    private void Close()
    {
        IsOpened = false;
        Closed.InvokeAsync(MyColor);
    }
}