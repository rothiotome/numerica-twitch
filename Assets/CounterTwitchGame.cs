using TMPro;
using TwitchChat;
using UnityEngine;

public class CounterTwitchGame : MonoBehaviour
{
    [SerializeField] private string channelName;
    [SerializeField] private TextMeshProUGUI usernameTMP;
    [SerializeField] private TextMeshProUGUI currentScoreTMP;
    [SerializeField] private TextMeshProUGUI maxScoreTMP;

    private int currentScore;
    private int currentMaxScore;

    private string lastUsername = "";
    private readonly string maxScoreKey = "maxScore";

    private void Start()
    {
        TwitchController.Login(channelName);
        TwitchController.onTwitchMessageReceived += OnTwitchMessageReceived;

        currentMaxScore = PlayerPrefs.GetInt(maxScoreKey);
        maxScoreTMP.SetText($"HIGH SCORE: {currentMaxScore}");
        usernameTMP.SetText("");
        ResetGame();
    }
    
    private void OnDestroy()
    {
        TwitchController.onTwitchMessageReceived -= OnTwitchMessageReceived;
    }

    private void GameLost()
    {
        ResetGame();
    }

    private void ResetGame()
    {
        lastUsername = "";
        currentScore = 0;
        currentScoreTMP.SetText(currentScore.ToString());
    }
    
    private void OnTwitchMessageReceived(string username, string message)
    {
        if(int.TryParse(message, out int response))
        {
            if (lastUsername.Equals(username)) return;
            
            if (response == currentScore + 1)
            {
                currentScore++;
                usernameTMP.SetText(username);
                currentScoreTMP.SetText(currentScore.ToString());
                lastUsername = username;
                if (currentScore > currentMaxScore)
                {
                    currentMaxScore = currentScore;
                    maxScoreTMP.SetText($"HIGH SCORE: {currentMaxScore}");
                    PlayerPrefs.SetInt(maxScoreKey, currentScore);
                }
            }
            else
            {
                if (currentScore != 0)
                {
                    usernameTMP.SetText($"<color=#00EAC0>Shame on </color>{username}<color=#00EAC0>!</color>");
                    GameLost();
                }

            }
        }
    }
}
