using BlazorColorPicker;

namespace BlazorColorPicker.UnitTests;

[TestClass]
public class PaletteChoiceTests
{
    public static IEnumerable<object[]> States =>
    [
        ["#FFE4E9", 100, 0],   // lightest red
        ["#2196F3", 100, 0],   // blue 500
        ["#2196F3", 140, 30],
        ["#7A1FA2", 0, -180],
        ["#FFB64D", 200, 180],
        ["#BDBDBD", 60, 90],   // grey
        ["#273238", 173, -45], // darkest blue grey
    ];

    [TestMethod]
    [DynamicData(nameof(States))]
    public void RoundTrip_State_Hex_Json_RestoresIdenticalState(string baseHex, int intensity, int hue)
    {
        var choice = PaletteChoice.Create(baseHex, intensity, hue);

        var restored = PaletteChoice.FromJson(choice.ToJson());

        Assert.AreEqual(choice, restored);
        Assert.AreEqual(choice.Hex, PaletteChoice.Create(baseHex, intensity, hue).Hex);
    }

    [TestMethod]
    public void NeutralGrid_ReproducesMaterialPalette()
    {
        var grid = PaletteColorMath.ComputeGrid(PaletteChoice.NeutralIntensity, PaletteChoice.NeutralHue);

        CollectionAssert.AreEqual(
            PaletteColorMath.DefaultColors.Select(c => c.ToUpperInvariant()).ToArray(),
            grid);
    }

    [TestMethod]
    public void Grey_IsNotAffectedByHue()
    {
        var neutral = PaletteChoice.Create("#9E9E9E", 100, 0);
        var shifted = PaletteChoice.Create("#9E9E9E", 100, 120);

        Assert.AreEqual(neutral.Hex, shifted.Hex);
    }

    [TestMethod]
    public void Grey_IsAffectedByIntensity()
    {
        var neutral = PaletteChoice.Create("#9E9E9E", 100, 0);
        var stronger = PaletteChoice.Create("#9E9E9E", 200, 0);

        Assert.AreNotEqual(neutral.Hex, stronger.Hex);
    }

    [TestMethod]
    public void FromHex_BaseColor_ReturnsNeutralSliders()
    {
        var choice = PaletteChoice.FromHex("#2196F3");

        Assert.IsNotNull(choice);
        Assert.AreEqual("#2196F3", choice.BaseHex);
        Assert.AreEqual(100, choice.Intensity);
        Assert.AreEqual(0, choice.Hue);
        Assert.AreEqual("#2196F3", choice.Hex);
    }

    [TestMethod]
    [DynamicData(nameof(States))]
    public void FromHex_AdjustedPaletteColor_FindsMatchingState(string baseHex, int intensity, int hue)
    {
        var source = PaletteChoice.Create(baseHex, intensity, hue);

        var found = PaletteChoice.FromHex(source.Hex);

        Assert.IsNotNull(found);
        PaletteColorMath.TryParseHex(source.Hex, out var a);
        PaletteColorMath.TryParseHex(found.Hex, out var b);
        Assert.IsTrue(PaletteColorMath.Distance(a, b) <= PaletteChoice.FromHexThreshold,
            $"{source} -> {found}");
    }

    [TestMethod]
    public void FromHex_OutOfPaletteColor_ReturnsNull()
    {
        // Very dark, saturated red: no cell + slider combination comes within the threshold.
        Assert.IsNull(PaletteChoice.FromHex("#300808"));
    }

    [TestMethod]
    public void FromHex_InvalidHex_ReturnsNull()
    {
        Assert.IsNull(PaletteChoice.FromHex(null));
        Assert.IsNull(PaletteChoice.FromHex("not a color"));
    }

    [TestMethod]
    public void FromJson_InvalidOrOutOfRange_ReturnsNull()
    {
        Assert.IsNull(PaletteChoice.FromJson(null));
        Assert.IsNull(PaletteChoice.FromJson("{oops"));
        Assert.IsNull(PaletteChoice.FromJson("""{"BaseHex":"#123456","Intensity":100,"Hue":0,"Hex":"#123456"}"""));
        Assert.IsNull(PaletteChoice.FromJson("""{"BaseHex":"#2196F3","Intensity":250,"Hue":0,"Hex":"#000000"}"""));
    }

    [TestMethod]
    public void Create_IsCaseInsensitive_AndNormalizesBaseHex()
    {
        var choice = PaletteChoice.Create("#2196f3");

        Assert.AreEqual("#2196F3", choice.BaseHex);
        Assert.AreEqual("#2196F3", choice.Hex);
    }

    [TestMethod]
    public void Create_ColorOutsidePalette_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() => PaletteChoice.Create("#123456"));
    }

    [TestMethod]
    public void Json_IsReadable()
    {
        var json = PaletteChoice.Create("#673BB7", 99, 61).ToJson();

        StringAssert.StartsWith(json, """{"BaseHex":"#673BB7","Intensity":99,"Hue":61,"Hex":"#""");
    }

    [TestMethod]
    public void Resolve_PrefersJson_ThenFallsBackToHex()
    {
        var stored = PaletteChoice.Create("#5D35B0", 150, -20);

        Assert.AreEqual(stored, PaletteChoice.Resolve(stored.ToJson(), "#2196F3"));

        var fromHex = PaletteChoice.Resolve(null, "#2196F3");
        Assert.IsNotNull(fromHex);
        Assert.AreEqual("#2196F3", fromHex.BaseHex);
    }

    [TestMethod]
    [DataRow(103, 100)]
    [DataRow(96, 100)]
    [DataRow(105, 105)]
    [DataRow(94, 94)]
    public void Snap_Intensity(int value, int expected)
        => Assert.AreEqual(expected, PaletteColorMath.Snap(value, 100, 4));

    [TestMethod]
    [DataRow(7, 0)]
    [DataRow(-7, 0)]
    [DataRow(8, 8)]
    [DataRow(-8, -8)]
    public void Snap_Hue(int value, int expected)
        => Assert.AreEqual(expected, PaletteColorMath.Snap(value, 0, 7));

    [TestMethod]
    public void TrackGradients_HavePlainEndsOf12px()
    {
        var gradient = PaletteColorMath.HueTrackGradient(PaletteColorMath.IndexOf("#2196F3"), 100);

        StringAssert.StartsWith(gradient, "linear-gradient(to right, ");
        StringAssert.Contains(gradient, "calc(12px + (100% - 24px) * 0)");
        StringAssert.Contains(gradient, "calc(12px + (100% - 24px) * 1)");
        StringAssert.EndsWith(gradient, " 100%)");
    }
}
