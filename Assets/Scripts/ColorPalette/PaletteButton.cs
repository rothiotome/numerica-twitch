using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaletteButton : MonoBehaviour
{
   [SerializeField] TextMeshProUGUI paletteNameTMP;
   [SerializeField] private Button button;

   public string paletteName { get; private set; }
   public void Initialize(string paletteName, PaletteSelector selector)
   {
      this.paletteName = paletteName;
      paletteNameTMP.SetText(paletteName);
      button.onClick.AddListener(() => selector.SetPalette(paletteName));
   }
   public void SetButtonState(bool newState)
   {
      button.interactable = newState;
   }

   public void Click()
   {
      button.onClick.Invoke();
   }
}
