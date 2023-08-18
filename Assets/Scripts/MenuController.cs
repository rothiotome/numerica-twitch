using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuController : DynamicColor, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] private GameObject panel;
    private TextMeshProUGUI tmp;

    private void Awake()
    {
        TryGetComponent(out tmp);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tmp.color = PaletteController.CurrentPalette.GetCustomColor(ColorType.Number).color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        panel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tmp.color = PaletteController.CurrentPalette.GetCustomColor(ColorType.Eclipse).color;
    }

    protected override void UpdateColor()
    {
        tmp.color = PaletteController.CurrentPalette.GetCustomColor(ColorType.Eclipse).color;
    }
}
