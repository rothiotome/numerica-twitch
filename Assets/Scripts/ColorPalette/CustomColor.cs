using System;
using UnityEngine;

[Serializable]
public class CustomColor
{
   public Color color;
   public string hex;
   public ColorType colorType;
   
   public void FillHex()
   {
      hex = "#" + ColorUtility.ToHtmlStringRGB(color);
   }
   
   public void FillColor()
   {
      Color newColor;
      if (ColorUtility.TryParseHtmlString(hex, out newColor))
      {
         color = newColor;
         return;
      }
      
      if(ColorUtility.TryParseHtmlString("#" + hex, out newColor))
      {
         color = newColor;
         return;
      }
      
      Debug.Log($"Unable to convert {hex} to Color");
   }

   public void FillColorType(ColorType colorType)
   {
      this.colorType = colorType;
   }
}
