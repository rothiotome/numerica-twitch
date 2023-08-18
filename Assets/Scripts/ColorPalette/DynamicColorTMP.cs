using TMPro;
using UnityEngine;

public class DynamicColorTMP : DynamicColor
{
    private TextMeshProUGUI myTextMeshProUGUI;
    [SerializeField] private ColorType color;

    private void Awake()
    {
        TryGetComponent(out myTextMeshProUGUI);
    }

    protected override void UpdateColor()
    {
        myTextMeshProUGUI.color = PaletteController.CurrentPalette.GetCustomColor(color).color;
    }
}
