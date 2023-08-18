using VerySimpleTwitchAPI;
using VerySimpleTwitchChat;
using UnityEngine;

public class CounterTwitchGame : MonoBehaviour
{
    
    private int currentScore;

    private string lastUsername = string.Empty;

    private int currentMaxScore;
    private readonly string maxScoreKey = "maxScore";

    private string currentMaxScoreUsername = "RothioTome";
    private readonly string maxScoreUsernameKey = "maxScoreUsername";

    private string lastUserIdVIPGranted;
    private readonly string lastUserIdVIPGrantedKey = "lastVIPGranted";

    private string nextPotentialVIP;

    [SerializeField] private GameObject startingCanvas;

    [SerializeField] private UIController uiController;

    private void Start()
    {
        Application.targetFrameRate = 30;

        TwitchChat.onTwitchMessageReceived += OnTwitchMessageReceived;
        TwitchChat.onChannelJoined += OnChannelJoined;
        PaletteController.OnPaletteUpdated += OnPaletteUpdated;

        currentMaxScore = PlayerPrefs.GetInt(maxScoreKey);
        currentMaxScoreUsername = PlayerPrefs.GetString(maxScoreUsernameKey, currentMaxScoreUsername);
        lastUserIdVIPGranted = PlayerPrefs.GetString(lastUserIdVIPGrantedKey, string.Empty);

        uiController.UpdateMaxScoreUI(currentMaxScore, currentMaxScoreUsername, CheckDisplayVip());
        uiController.UpdateCurrentScoreUI(currentScore, " ");
        ResetGame();
    }

    private bool CheckDisplayVip()
    {
        return (TwitchOAuth.Instance.IsVipEnabled() &&
                (!string.IsNullOrEmpty(nextPotentialVIP) || !string.IsNullOrEmpty(lastUserIdVIPGranted)));
    }

    private void OnDestroy()
    {
        TwitchChat.onTwitchMessageReceived -= OnTwitchMessageReceived;
        TwitchChat.onChannelJoined -= OnChannelJoined;
        PaletteController.OnPaletteUpdated -= OnPaletteUpdated;
    }

    private void OnPaletteUpdated()
    {
        uiController.UpdateCurrentScoreUI(currentScore, lastUsername);
        uiController.UpdateMaxScoreUI(currentMaxScore, currentMaxScoreUsername, CheckDisplayVip());
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
        uiController.UpdateCurrentScoreUI(currentScore, displayName);

        lastUsername = displayName;
        if (currentScore > currentMaxScore)
        {
            SetMaxScore(displayName, currentScore);
            HandleVIPStatusUpdate(chatter);
        }
    }

    private void HandleIncorrectResponse(string displayName, Chatter chatter)
    {
        if (currentScore != 0)
        {
            uiController.DisplayShameMessage(displayName);

            if (TwitchOAuth.Instance.IsVipEnabled())
            {
                if (lastUserIdVIPGranted.Equals(chatter.tags.userId))
                {
                    RemoveLastVIP();
                }

                HandleNextPotentialVIP();
            }

            HandleTimeout(chatter);
            uiController.UpdateMaxScoreUI(currentMaxScore, currentMaxScoreUsername, CheckDisplayVip());
            ResetGame();
        }
    }

    private void HandleNextPotentialVIP()
    {
        if (!string.IsNullOrEmpty(nextPotentialVIP))
        {
            if (nextPotentialVIP == "-1")
            {
                RemoveLastVIP();
            }
            else
            {
                if (!string.IsNullOrEmpty(lastUserIdVIPGranted))
                {
                    RemoveLastVIP();
                }

                GrantVIPToNextPotentialVIP();
            }

            nextPotentialVIP = string.Empty;
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

    private void GrantVIPToNextPotentialVIP()
    {
        TwitchOAuth.Instance.SetVIP(nextPotentialVIP, true);
        lastUserIdVIPGranted = nextPotentialVIP;
        PlayerPrefs.SetString(lastUserIdVIPGrantedKey, lastUserIdVIPGranted);
    }
    
    private void OnChannelJoined()
    {
        startingCanvas.SetActive(false);
    }

    public void ResetHighScore()
    {
        SetMaxScore("RothioTome", 0);
        RemoveLastVIP();
        ResetGame();
    }

    private void SetMaxScore(string username, int score)
    {
        currentMaxScore = score;
        currentMaxScoreUsername = username;
        PlayerPrefs.SetString(maxScoreUsernameKey, username);
        PlayerPrefs.SetInt(maxScoreKey, score);
        uiController.UpdateMaxScoreUI(currentMaxScore, currentMaxScoreUsername, CheckDisplayVip());
    }

    private void ResetGame()
    {
        lastUsername = "";
        currentScore = 0;
        uiController.UpdateCurrentScoreUI(currentScore);
    }
}