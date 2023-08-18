using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PaletteSelector : MonoBehaviour
{
    [SerializeField] private PaletteButton prefab;
    [SerializeField] private GameObject copyToCustom;
    private Button customButton;
    private List<PaletteButton> palettes = new List<PaletteButton>();
    [SerializeField] private GameObject colorSelectorMenu;

    private void Start()
    {
        foreach (var colorPaletteScriptable in PaletteController.GetPalettes())
        {
            PaletteButton palette = Instantiate(prefab, transform);
            palette.Initialize(colorPaletteScriptable.Key, this);
            palettes.Add(palette);
            if (palette.paletteName.Equals(PaletteController.CurrentPalette.name)) palette.SetButtonState(false);
        }
    }

    public void OnEnable()
    {
        LoadCustomPalette();
        PaletteController.OnPaletteUpdated += OnPaletteChanged;
    }

    public void OnDisable()
    {
        PersistCustomPalette();
        PaletteController.OnPaletteUpdated -= OnPaletteChanged;
    }

    public void OnPaletteChanged()
    {
        copyToCustom.SetActive(!PaletteController.CurrentPalette.name.Equals(PaletteController.customPaletteName));
    }

    public void CopyToCustom()
    {
        ColorPaletteScriptable palette = Instantiate(PaletteController.CurrentPalette);
        palette.name = PaletteController.customPaletteName;
        PaletteController.OverwriteCustomPalette(palette);
        palettes[0].Click();
    }

    public void PersistCustomPalette()
    {
        string customPaletteJson = Application.persistentDataPath + "/customPalette.json";

        File.WriteAllText(customPaletteJson,
            JsonUtility.ToJson(PaletteController.GetPalettes()[PaletteController.customPaletteName]));
    }

    public void LoadCustomPalette()
    {
        string customPaletteJson = Application.persistentDataPath + "/customPalette.json";

        if (File.Exists(customPaletteJson))
        {
            string json = File.ReadAllText(customPaletteJson);
            JsonUtility.FromJsonOverwrite(json, PaletteController.GetPalettes()[PaletteController.customPaletteName]);
        }
    }

    public void CloseCustomizeMenu()
    {
        colorSelectorMenu.SetActive(false);
    }

    public void SetPalette(string paletteName)
    {
        PaletteController.SetCurrentPalette(paletteName);
        palettes.ForEach(x => x.SetButtonState(x.paletteName != paletteName));
    }
}