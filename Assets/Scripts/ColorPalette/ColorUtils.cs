using System.Text;

public static class ColorUtils
{
    public static StringBuilder AppendColorTags(this StringBuilder str, string appendText, ColorType color)
    {
        return str.Append($"<color={PaletteController.CurrentPalette.GetCustomColor(color).hex}>{appendText}</color>");
    }
    
    public static string GetTaggedColorString(string str, ColorType color)
    {
        return $"<color={PaletteController.CurrentPalette.GetCustomColor(color).hex}>{str}</color>";
    }
}
