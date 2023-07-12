using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class TwitchVersionMessage : MonoBehaviour, IPointerClickHandler
{

#if UNITY_STANDALONE_WIN
    private string newVersionAPI = "https://itch.io/api/1/x/wharf/latest?game_id=2145888&channel_name=win";
#elif UNITY_STANDALONE_LINUX	
    private string newVersionAPI = "https://itch.io/api/1/x/wharf/latest?game_id=2145888&channel_name=linux";
#else 
    private string newVersionAPI = "https://itch.io/api/1/x/wharf/latest?game_id=2145888&channel_name=win";
#endif

    private TextMeshProUGUI tmp;
    private void Awake()
    {
        TryGetComponent(out tmp);
        tmp.SetText($"v{Application.version} A game by RothioTome. Art by PSuzume");
    }

    private IEnumerator Start()
    {
        using (UnityWebRequest webRequest =
               UnityWebRequest.Get(newVersionAPI))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    Debug.Log("Received: " + webRequest.downloadHandler.text);
                    if (webRequest.downloadHandler.text.Contains("latest"))
                    {
                        ShowNewVersionMessage(webRequest.downloadHandler.text.Split('"')[3]);
                    }
                    break;
            }
        }
    }

    private void ShowNewVersionMessage(string versionNumber)
    {
        if (Application.version == versionNumber) return;
        tmp.SetText(
            $"New version {versionNumber} available! Download it from <link=\"https://rothiotome.itch.io/numerica/\">https://rothiotome.itch.io/numerica</link>");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Application.OpenURL("https://rothiotome.itch.io/numerica");
    }
}
