using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MockColor : MonoBehaviour
{
    [SerializeField] private ColorType colorType;
    [SerializeField] private Transform colorSelectorsParent;

    private ColorSelector myColorSelector;
    
    private void Start()
    {
        PaletteController.OnPaletteUpdated += UpdateColor;
        UpdateColor();
        Button button = gameObject.AddComponent<Button>();
        button.onClick.AddListener(SelectInputField);
        ColorBlock colors = button.colors;
        colors.highlightedColor = Color.gray;
        button.colors = colors;
    }

    public void SelectInputField()
    {
        if (myColorSelector == null)
        {
            myColorSelector = colorSelectorsParent.GetComponentsInChildren<ColorSelector>()
                .First(x => colorType == x.GetColorType);
        }

        myColorSelector.FocusInputField();
    }

    void UpdateColor()
    {
        if (TryGetComponent(out TextMeshProUGUI tmp))
        {
            tmp.color = PaletteController.CurrentPalette.GetCustomColor(colorType).color;
        }
        else if (TryGetComponent(out Image image))
        {
            image.color = PaletteController.CurrentPalette.GetCustomColor(colorType).color;
        }
    }
}