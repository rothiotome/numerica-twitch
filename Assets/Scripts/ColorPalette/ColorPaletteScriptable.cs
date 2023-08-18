using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "ScriptableObjects/ColorPalette")]
public class ColorPaletteScriptable : ScriptableObject
{
    public CustomColor background;
    public CustomColor eclipse;
    public CustomColor number;
    public CustomColor by;
    public CustomColor highScoreMessage;
    public CustomColor highScoreNumber;
    public CustomColor shameOnMessage;
    public CustomColor shameOnUsername;
    public CustomColor highScoreUsername;
     
    public void FillHex()
    {
        background.FillHex();
        eclipse.FillHex();
        number.FillHex();
        by.FillHex();
        highScoreMessage.FillHex();
        highScoreNumber.FillHex();
        shameOnMessage.FillHex();
        shameOnUsername.FillHex();
        highScoreUsername.FillHex();
    }
    
    public void FillColor()
    {
        background.FillColor();
        eclipse.FillColor();
        number.FillColor();
        by.FillColor();
        highScoreMessage.FillColor();
        highScoreNumber.FillColor();
        shameOnMessage.FillColor();
        shameOnUsername.FillColor();
        highScoreUsername.FillColor();
    }

    public void SetPalette()
    {
        PaletteController.CurrentPalette = this;
    }

    public CustomColor GetCustomColor(ColorType colorType)
    {
        switch (colorType)
        {
            case ColorType.Background:
                return background;
            case ColorType.Eclipse:
                return eclipse;
            case ColorType.Number:
                return number;
            case ColorType.By:
                return by;
            case ColorType.HighScoreMessage:
                return highScoreMessage;
            case ColorType.HighScoreNumber:
                return highScoreNumber;
            case ColorType.ShameOnMessage:
                return shameOnMessage;
            case ColorType.ShameOnUsername:
                return shameOnUsername;
            case ColorType.HighScoreUsername:
                return highScoreUsername;
            default:
                throw new ArgumentOutOfRangeException(nameof(colorType), colorType, null);
        }
    }

    public void SetCustomColor(ColorType colorType, CustomColor newColor)
    {
        switch (colorType)
        {
            case ColorType.Background:
                background = newColor;
                break;
            case ColorType.Eclipse:
                eclipse = newColor;
                break;
            case ColorType.Number:
                number = newColor;
                break;
            case ColorType.By:
                by = newColor;
                break;
            case ColorType.HighScoreMessage:
                highScoreMessage = newColor;
                break;
            case ColorType.HighScoreNumber:
                highScoreNumber = newColor;
                break;
            case ColorType.ShameOnMessage:
                shameOnMessage = newColor;
                break;
            case ColorType.ShameOnUsername:
                shameOnUsername = newColor;
                break;
            case ColorType.HighScoreUsername:
                highScoreUsername = newColor;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(colorType), colorType, null);
        }
        PaletteController.OnPaletteUpdated.Invoke();
    }
}