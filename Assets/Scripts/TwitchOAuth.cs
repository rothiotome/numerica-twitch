using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TwitchChat;
using UnityEngine;
using UnityEngine.Networking;

public class TwitchOAuth : MonoBehaviour
{
    private readonly string twitchValidateUrl = "https://id.twitch.tv/oauth2/validate";
    private readonly string twitchBanUrl = "https://api.twitch.tv/helix/moderation/bans";
    private readonly string twitchAuthUrl = "https://id.twitch.tv/oauth2/authorize";
    private readonly string twitchVipUrl = "https://api.twitch.tv/helix/channels/vips";

    private readonly string twitchRedirectUrl = "http://localhost:8080/";
    private readonly string loginSuccessUrl = "https://rociotome.com/success-numerica-login";
    private readonly string loginFailUrl = "https://rociotome.com/fail-numerica-login";
    
    private string twitchAuthStateVerify;
    private string authToken = "";

    private bool oauthTokenRetrieved;

    private string userId;
    private string channelName;
    
    private HttpClient httpClient = new HttpClient();
    private HttpListener httpListener;
    
    private bool enableTimeout = true;
    private bool enableVip = true;
    private bool enableModImmunity = false;
    private int timeoutMultiplier = 10;

    #region Singleton
    public static TwitchOAuth Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    
    #endregion

    private void Update()
    {
        if (!oauthTokenRetrieved) return;
        
        TwitchController.Login(channelName, new TwitchLoginInfo(channelName, authToken));
        oauthTokenRetrieved = false;
        InvokeRepeating(nameof(ValidateToken), 3600, 3600);
    }
    
    public void SetTimeoutOption(bool state)
    {
        enableTimeout = state;
    }

    public void SetVipOption(bool state)
    {
        enableVip = state;
    }

    public void SetModImmunityOption(bool state)
    {
        enableModImmunity = state;
    }

    public bool IsVipEnabled()
    {
        return enableVip;
    }
    
    public bool IsModImmunityEnabled()
    {
        return enableModImmunity;
    }
    
    public void SetTimeoutMultiplayer(string multiplier)
    {
        int.TryParse(multiplier, out timeoutMultiplier);
        if (timeoutMultiplier < 1) timeoutMultiplier = 10;
    }

    public bool Timeout(string targetUserId, int failedNumber)
    {
        if (enableTimeout) timeout(targetUserId, failedNumber);
        
        return enableTimeout;
    }

    public bool SetVIP(string targetUserId, bool state)
    {
        if (enableVip) setVIP(targetUserId, state);
        return enableVip;
    }
    
    /// <summary>
    /// Starts the Twitch OAuth flow by constructing the Twitch auth URL based on the scopes you want/need.
    /// </summary>
    public void InitiateTwitchAuth()
    {
        List<string> scopes = new List<string>{"chat:read"};
        
        if (enableTimeout)
        {
            scopes.Add("moderator:manage:banned_users");
        }

        if (enableVip)
        {
            scopes.Add("channel:manage:vips");
        }

        // generate something for the "state" parameter.
        // this can be whatever you want it to be, it's gonna be "echoed back" to us as is and should be used to
        // verify the redirect back from Twitch is valid.
        twitchAuthStateVerify = ((Int64)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();

        // query parameters for the Twitch auth URL
        var s = "client_id=" + Secrets.CLIENT_ID + "&" +
                "redirect_uri=" + UnityWebRequest.EscapeURL(twitchRedirectUrl) + "&" +
                "state=" + twitchAuthStateVerify + "&" +
                "response_type=token" + "&" +
                "scope=" + String.Join("+", scopes);

        // start our local webserver to receive the redirect back after Twitch authenticated
        StartLocalWebserver();

        // open the users browser and send them to the Twitch auth URL
        Application.OpenURL(twitchAuthUrl + "?" + s);
    }

    /// <summary>
    /// Opens a simple "webserver" like thing on localhost:8080 for the auth redirect to land on.
    /// Based on the C# HttpListener docs: https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistener
    /// </summary>
    private void StartLocalWebserver()
    {
        httpListener = new HttpListener();

        httpListener.Prefixes.Add(twitchRedirectUrl);
        httpListener.Start();
        httpListener.BeginGetContext(IncomingHttpRequest, httpListener);
    }

    /// <summary>
    /// Handles the incoming HTTP request
    /// </summary>
    /// <param name="result"></param>
    private void IncomingHttpRequest(IAsyncResult result)
    {
        // fetch the context object
        HttpListenerContext httpContext = httpListener.EndGetContext(result);

        // the context object has the request object for us, that holds details about the incoming request
        HttpListenerRequest httpRequest = httpContext.Request;

        string[] tokens = httpRequest.QueryString.AllKeys;

        if (tokens.Contains("access_token"))
        {
            authToken = httpRequest.QueryString.Get("access_token");
            string state = httpRequest.QueryString.Get("state");

            if (state == twitchAuthStateVerify)
            {
                string responseString = $"<html><body><script>window.location.replace(\"{loginSuccessUrl}\");</script></body></html>";
                ValidateToken(true);
                SendResponse(httpContext, responseString);

                httpListener.Stop();
            }
            else
            {
                string responseString = $"<html><body><script>window.location.replace(\"{loginFailUrl}\");</script></body></html>";
                SendResponse(httpContext, responseString);
            
                httpListener.Stop();
            }
        }else if (tokens.Contains("error"))
        {
            string responseString = $"<html><body><script>window.location.replace(\"{loginFailUrl}\");</script></body></html>";
            SendResponse(httpContext, responseString);
            
            httpListener.Stop();
        }
        else
        {
            string responseString = "<html><head><meta http-equiv='cache-control' content='no-cache'><meta http-equiv='expires' content='0'> <meta http-equiv='pragma' content='no-cache'></head><body><script>var link = window.location.toString(); link = link.replace('#','?'); window.location.replace(link);</script></body></html>";
            SendResponse(httpContext, responseString);
            httpListener.BeginGetContext(IncomingHttpRequest, httpListener);
        }
    }

    private void SendResponse(HttpListenerContext httpContext, string responseString)
    {
        HttpListenerResponse httpResponse = httpContext.Response;

        // build a response to send an "ok" back to the browser for the user to see
        httpResponse = httpContext.Response;
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);

        // send the output to the client browser
        httpResponse.ContentLength64 = buffer.Length;
        System.IO.Stream output = httpResponse.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
    }

    private async Task ValidateToken(bool shouldConnectChat = false)
    {
        string apiResponseJson = await CallApi(twitchValidateUrl);
        ApiValidateResponse apiResponseData = JsonUtility.FromJson<ApiValidateResponse>(apiResponseJson);

        userId = apiResponseData.user_id;
        channelName = apiResponseData.login;
        
        if(shouldConnectChat) oauthTokenRetrieved = true;
    }
    
    private async Task setVIP(string targetUserId, bool state)
    {
        string apiUrl = twitchVipUrl +
                        "?user_id=" + targetUserId +
                        "&broadcaster_id=" + userId;

        await CallApi(apiUrl, state ? "POST" : "DELETE");
    }
    
    private async Task timeout(string targetUserId, int failedNumber)
    {
        string apiUrl = twitchBanUrl +
                        "?broadcaster_id=" + userId +
                        "&moderator_id=" + userId;
        
        string body = $"{{\"data\": {{\"user_id\":\"{targetUserId}\",\"duration\":{failedNumber*timeoutMultiplier}}}}}";

        await CallApi(apiUrl, "POST", body);
    }

    private async Task<string> CallApi(string endpoint, string method = "GET", string body = "", string[] headers = null)
    {

        int retries = 0;
        
        httpClient.BaseAddress = null;
        httpClient.DefaultRequestHeaders.Clear();

        HttpMethod httpMethod = new HttpMethod(method.ToUpperInvariant());
        HttpRequestMessage httpRequest = new HttpRequestMessage(httpMethod, endpoint);

        if (!string.IsNullOrEmpty(body))
        {
            httpRequest.Content = new StringContent(body, Encoding.UTF8, "application/json");
        }

        if (!string.IsNullOrEmpty(authToken))
        {
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        }

        if (!string.IsNullOrEmpty(Secrets.CLIENT_ID))
        {
            httpRequest.Headers.TryAddWithoutValidation("Client-Id", Secrets.CLIENT_ID);
        }

        httpRequest.Headers.TryAddWithoutValidation("Content-Type", "application/json");

        if (headers != null)
        {
            foreach (string header in headers)
            {
                string[] headerParts = header.Split(':');
                if (headerParts.Length >= 2 && !string.IsNullOrWhiteSpace(headerParts[0]) &&
                    !string.IsNullOrWhiteSpace(headerParts[1]))
                {
                    httpRequest.Headers.TryAddWithoutValidation(headerParts[0].Trim(), headerParts[1].Trim());
                }
            }
        }

        while (retries < 3)
        {
            Tuple<HttpStatusCode, string> response = await HttpCall(httpRequest);

            switch (response.Item1)
            {
                case HttpStatusCode.OK: 
                    return response.Item2;
                case HttpStatusCode.Unauthorized:
                    Debug.Log("Unauthorized");
                    break;
                default:
                    break;
            }

            retries++;
        }

        return string.Empty;
    }

    private async Task<Tuple<HttpStatusCode, string>> HttpCall(HttpRequestMessage httpRequest)
    {
        HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest).ConfigureAwait(false);
        httpResponse.EnsureSuccessStatusCode();

        string httpResponseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        return new Tuple<HttpStatusCode, string>(httpResponse.StatusCode, httpResponseContent);
    }

    private void OnApplicationQuit()
    {
        httpListener?.Stop();
        httpListener?.Abort();
    }
}