using UnityEngine;

public abstract class DynamicColor : MonoBehaviour
{
   private void OnEnable()
   {
      PaletteController.OnPaletteUpdated += OnPaletteUpdated;
      UpdateColor();
   }
   
   private void OnDisable()
   {
      PaletteController.OnPaletteUpdated -= OnPaletteUpdated;
   }
   
   private void OnPaletteUpdated()
   {
      UpdateColor();
   }
   
   protected abstract void UpdateColor();
}
