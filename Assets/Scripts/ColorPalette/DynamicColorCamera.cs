using UnityEngine;

public class DynamicColorCamera : DynamicColor
{
    private Camera myCamera;
    [SerializeField] private ColorType color;

    private void Awake()
    {
        TryGetComponent(out myCamera);
    }

    protected override void UpdateColor()
    {
        myCamera.backgroundColor = PaletteController.CurrentPalette.GetCustomColor(color).color;
    }
}
