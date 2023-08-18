using UnityEngine;
using UnityEngine.UI;

public class DynamicColorImage : DynamicColor
{
    private Image myImage;
    [SerializeField] private ColorType color;

    private void Awake()
    {
        TryGetComponent(out myImage);
    }

    protected override void UpdateColor()
    {
        myImage.color = PaletteController.CurrentPalette.GetCustomColor(color).color;
    }
}