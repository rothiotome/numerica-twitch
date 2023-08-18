using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ColorSelector : MonoBehaviour
{
    [SerializeField] private ColorType colorType;
    [SerializeField] private TMP_InputField inputTextField;
    [SerializeField] private Image sampleImage;
    [SerializeField] private TextMeshProUGUI colorName;

    public ColorType GetColorType => colorType;

    private void Start()
    {
        UpdateColor();
        inputTextField.onValueChanged.AddListener(NewPossibleColor);
    }

    private void NewPossibleColor(string newColor)
    {
        CustomColor newCustomColor = new CustomColor();
        if (!ColorUtility.TryParseHtmlString(newColor, out newCustomColor.color))
        {
            newCustomColor.color = Color.black;
        }
        
        newCustomColor.colorType = colorType;
        newCustomColor.hex = newColor;
        PaletteController.CurrentPalette.SetCustomColor(colorType, newCustomColor);
    }

    public void OnEnable()
    {
        PaletteController.OnPaletteUpdated += UpdateColor;
    }

    public void OnDisable()
    {
        PaletteController.OnPaletteUpdated -= UpdateColor;
    }

    private void UpdateColor()
    {
        CustomColor color = PaletteController.CurrentPalette.GetCustomColor(colorType);
        inputTextField.interactable = (PaletteController.CurrentPalette.name.Equals(PaletteController.customPaletteName)) ;
        colorName.SetText(color.colorType.ToString());
        inputTextField.SetTextWithoutNotify(color.hex);
        sampleImage.color = color.color;
    }

    public void FocusInputField()
    {
        inputTextField.Select();
    }
}
