using TMPro;
using TwitchChat;
using UnityEngine;
using Application = UnityEngine.Device.Application;

public class CounterTwitchGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI usernameTMP;
    [SerializeField] private TextMeshProUGUI currentScoreTMP;
    [SerializeField] private TextMeshProUGUI maxScoreTMP;

    private int currentScore;
    private int currentMaxScore;

    private string currentMaxScoreUsername = "RothioTome";

    private string lastUsername = "";
    private readonly string maxScoreKey = "maxScore";
    private readonly string maxScoreUsernameKey = "maxScoreUsername";
    private readonly string lastUserIdVIPGrantedKey = "lastVIPGranted";

    private string lastUserIdVIPGranted;
    private string nextPotentialVIP;
    
    [SerializeField] private GameObject startingCanvas;

    private void Start()
    {
        Application.targetFrameRate = 30;
        
        TwitchController.onTwitchMessageReceived += OnTwitchMessageReceived;
        TwitchController.onChannelJoined += OnChannelJoined;
        
        currentMaxScore = PlayerPrefs.GetInt(maxScoreKey);
        currentMaxScoreUsername = PlayerPrefs.GetString(maxScoreUsernameKey, "RothioTome");
        lastUserIdVIPGranted = PlayerPrefs.GetString(lastUserIdVIPGrantedKey, "");
        
        SetHighScore(currentMaxScore, currentMaxScoreUsername);
        usernameTMP.SetText("");
        ResetGame();
    }

    private void OnDestroy()
    {
        TwitchController.onTwitchMessageReceived -= OnTwitchMessageReceived;
        TwitchController.onChannelJoined -= OnChannelJoined;
    }
    
    private void OnTwitchMessageReceived(Chatter chatter)
    {
        if (!int.TryParse(chatter.message, out int response)) return;

        string displayName = chatter.IsDisplayNameFontSafe() ? chatter.tags.displayName : chatter.login;

        if (lastUsername.Equals(displayName)) return;

        if (response == currentScore + 1) HandleCorrectResponse(displayName, chatter);
        else HandleIncorrectResponse(displayName, chatter);

    }

    private void HandleCorrectResponse(string displayName, Chatter chatter)
    {
        currentScore++;
        usernameTMP.SetText(displayName);
        currentScoreTMP.SetText(currentScore.ToString());
        lastUsername = displayName;
        if (currentScore > currentMaxScore)
        {
            currentMaxScore = currentScore;
            currentMaxScoreUsername = displayName;
            PlayerPrefs.SetString(maxScoreUsernameKey, displayName);
            PlayerPrefs.SetInt(maxScoreKey, currentScore);
            
            HandleVIPStatusUpdate(chatter);
            SetHighScore(currentMaxScore, displayName);
        }
    }

    private void HandleIncorrectResponse(string displayName, Chatter chatter)
    {
        if (currentScore != 0)
        {
            DisplayShameMessage(displayName);

            if (TwitchOAuth.Instance.IsVipEnabled())
            {
                if (lastUserIdVIPGranted.Equals(chatter.tags.userId))
                {
                    RemoveLastVIP();
                }
                HandleNextPotentialVIP();
            }

            HandleTimeout(chatter);
            SetHighScore(currentMaxScore, currentMaxScoreUsername);
            ResetGame();
        }
    }
    
    private void HandleNextPotentialVIP()
    {
        if (!string.IsNullOrEmpty(nextPotentialVIP))
        {
            if (nextPotentialVIP == "-1")
            {
                if (!string.IsNullOrEmpty(lastUserIdVIPGranted))
                {
                    RemoveLastVIP();
                }
            }
            else
            {
                TwitchOAuth.Instance.SetVIP(nextPotentialVIP, true);
                if (!string.IsNullOrEmpty(lastUserIdVIPGranted))
                {
                    RemoveLastVIP();
                }

                lastUserIdVIPGranted = nextPotentialVIP;
            }

            nextPotentialVIP = "";
        }
    }

    private void HandleTimeout(Chatter chatter)
    {
        if (TwitchOAuth.Instance.IsModImmunityEnabled())
        {
            if (!chatter.HasBadge("moderator"))
            {
                TwitchOAuth.Instance.Timeout(chatter.tags.userId, currentScore);
            }
        }
        else
        {
            TwitchOAuth.Instance.Timeout(chatter.tags.userId, currentScore);
        }
    }
    
    private void HandleVIPStatusUpdate(Chatter chatter)
    {
        if (TwitchOAuth.Instance.IsVipEnabled())
        {
            if (!chatter.tags.HasBadge("vip"))
            {
                nextPotentialVIP = chatter.tags.userId;
            }
            else if (chatter.tags.userId == lastUserIdVIPGranted)
            {
                nextPotentialVIP = "";
            }
            else
            {
                nextPotentialVIP = "-1";
            }
        }
    }
    
    private void RemoveLastVIP()
    {
        TwitchOAuth.Instance.SetVIP(lastUserIdVIPGranted, false);
        lastUserIdVIPGranted = "";
        PlayerPrefs.SetString(lastUserIdVIPGrantedKey, lastUserIdVIPGranted);
    }

    private void SetHighScore(int score, string username)
    {
        if (TwitchOAuth.Instance.IsVipEnabled())
        {
            if (!string.IsNullOrEmpty(nextPotentialVIP))
            {
                maxScoreTMP.SetText($"HIGH SCORE: {score}\nby <color=#00EAC0><sprite=0>{username}</color>");
            }else if (string.IsNullOrEmpty(lastUserIdVIPGranted))
            {
                maxScoreTMP.SetText($"HIGH SCORE: {score}\nby <color=#00EAC0>{username}</color>");
            }
            else
            {
                maxScoreTMP.SetText($"HIGH SCORE: {score}\nby <color=#00EAC0><sprite=0>{username}</color>");
            }
        }
        else
        {
            maxScoreTMP.SetText($"HIGH SCORE: {score}\nby <color=#00EAC0>{username}</color>");
        }
    }
    
    private void DisplayShameMessage(string displayName)
    {
        usernameTMP.SetText($"<color=#00EAC0>Shame on </color>{displayName}<color=#00EAC0>!</color>");
    }

    private void OnChannelJoined()
    {
        startingCanvas.SetActive(false);
    }
    
    public void ResetHighScore()
    {
        currentMaxScore = 0;
        PlayerPrefs.SetString(maxScoreUsernameKey, "RothioTome");
        PlayerPrefs.SetInt(maxScoreKey, currentMaxScore);
        SetHighScore(0, "RothioTome");
        RemoveLastVIP();
        ResetGame();
    }
    
    private void ResetGame()
    {
        lastUsername = "";
        currentScore = 0;
        currentScoreTMP.SetText(currentScore.ToString());
    }
}
