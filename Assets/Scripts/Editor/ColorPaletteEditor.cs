
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColorPaletteScriptable))]
public class ColorPaletteEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ColorPaletteScriptable myScript = (ColorPaletteScriptable)target;
        
        if (GUILayout.Button("Fill Hex"))
        {
            myScript.FillHex();
            EditorUtility.SetDirty(myScript);
        }
        
        if (GUILayout.Button("Fill Color"))
        {
            myScript.FillColor();
            EditorUtility.SetDirty(myScript);
        }

        if (GUILayout.Button("Update type"))
        {
            myScript.background.colorType = ColorType.Background;
            myScript.eclipse.colorType = ColorType.Eclipse;
            myScript.number.colorType = ColorType.Number;
            myScript.by.colorType = ColorType.By;
            myScript.highScoreNumber.colorType = ColorType.HighScoreNumber;
            myScript.highScoreMessage.colorType = ColorType.HighScoreMessage;
            myScript.shameOnMessage.colorType = ColorType.ShameOnMessage;
            myScript.shameOnUsername.colorType = ColorType.ShameOnUsername;
            myScript.highScoreUsername.colorType = ColorType.HighScoreUsername;
            EditorUtility.SetDirty(myScript);
        }

        if (GUILayout.Button("Try Palette"))
        {
            myScript.FillColor();
            myScript.FillHex();
            myScript.SetPalette();
            EditorUtility.SetDirty(myScript);
        }
    }
}
