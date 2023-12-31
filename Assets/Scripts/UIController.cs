using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI usernameTMP;
    [SerializeField] private TextMeshProUGUI currentScoreTMP;
    [SerializeField] private TextMeshProUGUI maxScoreTMP;

    StringBuilder scoreText = new StringBuilder();
    StringBuilder shameText = new StringBuilder();

    private void OnEnable()
    {
        LoadCustomPalette();
    }

    public void UpdateMaxScoreUI(int currentMaxScore, string currentMaxScoreUsername, bool addVIPLogo)
    {
        scoreText.Clear();
        scoreText.AppendColorTags("HIGH SCORE: ", ColorType.HighScoreMessage)
            .AppendColorTags(currentMaxScore + "\n", ColorType.HighScoreNumber)
            .AppendColorTags("by ", ColorType.By)
            .AppendColorTags(addVIPLogo ? "<sprite=0>" + currentMaxScoreUsername : currentMaxScoreUsername,
                ColorType.HighScoreUsername);

        maxScoreTMP.SetText(scoreText);
    }

    public void UpdateCurrentScoreUI(int score, string username = "")
    {
        if (!string.IsNullOrEmpty(username))
            usernameTMP.SetText(ColorUtils.GetTaggedColorString(username, ColorType.ShameOnUsername));
        currentScoreTMP.SetText(score.ToString());
    }

    public void DisplayShameMessage(string displayName)
    {
        shameText.Clear();
        shameText.AppendColorTags("Shame on ", ColorType.ShameOnMessage)
            .AppendColorTags(displayName, ColorType.ShameOnUsername)
            .AppendColorTags("!", ColorType.ShameOnMessage);
        usernameTMP.SetText(shameText);
    }
    
    public void LoadCustomPalette()
    {
        string customPaletteJson = Application.persistentDataPath + "/customPalette.json";

        if (File.Exists(customPaletteJson))
        {
            string json = File.ReadAllText(customPaletteJson);
            JsonUtility.FromJsonOverwrite(json, PaletteController.GetPalettes()[PaletteController.customPaletteName]);
            if(PaletteController.CurrentPalette.name.Equals(PaletteController.customPaletteName)) PaletteController.OnPaletteUpdated.Invoke();
        }
    }
}