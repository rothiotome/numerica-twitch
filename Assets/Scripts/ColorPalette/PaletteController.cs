using System;
using System.Collections.Generic;
using UnityEngine;

public static class PaletteController
{
    private static Dictionary<string, ColorPaletteScriptable> Palettes;

    private static ColorPaletteScriptable currentPalette;

    public static readonly string customPaletteName = "0 Custom";

    private static string path = "ColorPalettes/";

    public static Action OnPaletteUpdated;

    private static string currentPalettePlayerPrefsKey = "CurrentPalette";

    public static Dictionary<string, ColorPaletteScriptable> GetPalettes()
    {
        if (Palettes == null)
        {
            Palettes = new Dictionary<string, ColorPaletteScriptable>();
            ColorPaletteScriptable[] palettes = Resources.LoadAll<ColorPaletteScriptable>(path);
            foreach (ColorPaletteScriptable palette in palettes)
            {
                Palettes.Add(palette.name, palette);
            }
        }

        return Palettes;
    }

    public static ColorPaletteScriptable CurrentPalette
    {
        get
        {
            if (currentPalette == null)
            {
                if (!SetCurrentPalette(PlayerPrefs.GetString(currentPalettePlayerPrefsKey, customPaletteName)))
                {
                    SetCurrentPalette(customPaletteName);
                }
            }

            return currentPalette;
        }
        set
        {
            if (value == currentPalette) return;
            currentPalette = value;
            OnPaletteUpdated?.Invoke();
        }
    }

    public static void OverwriteCustomPalette(ColorPaletteScriptable newCustom)
    {
        GetPalettes()[customPaletteName] = newCustom;
    }

    public static bool SetCurrentPalette(string name)
    {
        bool response;
        if (GetPalettes().TryGetValue(name, out ColorPaletteScriptable newColorPalette))
        {
            CurrentPalette = newColorPalette;
            PlayerPrefs.SetString(currentPalettePlayerPrefsKey, newColorPalette.name);
            response = true;
        }
        else
        {
            response = false;
            Debug.Log($"Can't find palette with name {name}");
        }

        return response;
    }
}