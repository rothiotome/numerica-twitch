/*
 * Simple Twitch OAuth flow example
 * by HELLCAT
 *
 * At first glance, this looks like more than it actually is.
 * It's really no rocket science, promised! ;-)
 * And for any further questions contact me directly or on the Twitch-Developers discord.
 *
 * 🐦 https://twitter.com/therealhellcat
 * 📺 https://www.twitch.tv/therealhellcat
 * 
 * Heavily modified by twitch.tv/RothioTome ✌️
 */
using System;
using System.Collections.Generic;
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
    private readonly string twitchRefreshTokenUrl = "https://id.twitch.tv/oauth2/token";
    private readonly string twitchAuthUrl = "https://id.twitch.tv/oauth2/authorize";
    private readonly string twitchVipUrl = "https://api.twitch.tv/helix/channels/vips";

    private string twitchRedirectUrl = "http://localhost:8080/";
    private string _twitchAuthStateVerify;
    private string _authToken = "";
    private string _refreshToken = "";

    private bool oauthTokenRetreived;

    private string userId;
    private string channelName;
    
    private HttpClient _httpClient = new HttpClient();

    private bool enableTimeout = true;
    private bool enableVip = true;
    private bool enableModImmunity = false;
    private int timeoutMultiplier = 10;

    private HttpListener httpListener;
    
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
    private void Update()
    {
        if (!oauthTokenRetreived) return;
        
        TwitchController.Login(channelName, new TwitchLoginInfo(channelName, _authToken));
        oauthTokenRetreived = false;
        InvokeRepeating("ValidateToken", 3600, 3600);
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
        _twitchAuthStateVerify = ((Int64)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();

        // query parameters for the Twitch auth URL
        var s = "client_id=" + Secrets.CLIENT_ID + "&" +
                "redirect_uri=" + UnityWebRequest.EscapeURL(twitchRedirectUrl) + "&" +
                "state=" + _twitchAuthStateVerify + "&" +
                "response_type=code&" +
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
        string code;
        string state;

        HttpListenerContext httpContext;
        HttpListenerRequest httpRequest;
        HttpListenerResponse httpResponse;
        string responseString;

        // get back the reference to our http listener
        // HttpListener httpListener = (HttpListener)result.AsyncState;

        // fetch the context object
        httpContext = httpListener.EndGetContext(result);

        // if we'd like the HTTP listener to accept more incoming requests, we'd just restart the "get context" here:
        // httpListener.BeginGetContext(new AsyncCallback(IncomingHttpRequest),httpListener);
        // however, since we only want/expect the one, single auth redirect, we don't need/want this, now.
        // but this is what you would do if you'd want to implement more (simple) "webserver" functionality
        // in your project.

        // the context object has the request object for us, that holds details about the incoming request
        httpRequest = httpContext.Request;

        code = httpRequest.QueryString.Get("code");
        state = httpRequest.QueryString.Get("state");

        // check that we got a code value and the state value matches our remembered one
        if ((code.Length > 0) && (state == _twitchAuthStateVerify))
        {
            // if all checks out, use the code to exchange it for the actual auth token at the API
            GetTokenFromCode(code);
        }

        // build a response to send an "ok" back to the browser for the user to see
        httpResponse = httpContext.Response;
        responseString = "<html><body><b>DONE!</b><br>(You can close this tab/window now)</body></html>";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

        // send the output to the client browser
        httpResponse.ContentLength64 = buffer.Length;
        System.IO.Stream output = httpResponse.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();

        // the HTTP listener has served it's purpose, shut it down
        httpListener.Stop();
    }

    /// <summary>
    /// Makes the API call to exchange the received code for the actual auth token
    /// </summary>
    /// <param name="code">The code parameter received in the callback HTTP request</param>
    private async Task GetTokenFromCode(string code)
    {
        string apiUrl;
        string apiResponseJson;
        ApiCodeTokenResponse apiResponseData;

        // construct full URL for API call
        apiUrl = twitchRefreshTokenUrl +
                 "?client_id=" + Secrets.CLIENT_ID +
                 "&client_secret=" + Secrets.CLIENT_SECRET +
                 "&code=" + code +
                 "&grant_type=authorization_code" +
                 "&redirect_uri=" + UnityWebRequest.EscapeURL(twitchRedirectUrl);

        // make the call!
        apiResponseJson = await CallApi(apiUrl, "POST");

        // parse the return JSON into a more usable data object
        apiResponseData = JsonUtility.FromJson<ApiCodeTokenResponse>(apiResponseJson);

        // fetch the token from the response data
        _authToken = apiResponseData.access_token;
        _refreshToken = apiResponseData.refresh_token;

        ValidateToken();
        
    }

    private async Task ValidateToken()
    {
        string apiUrl;
        string apiResponseJson;
        apiUrl = twitchValidateUrl;
        ApiValidateResponse apiResponseData;

        apiResponseJson = await CallApi(apiUrl, "GET");

        apiResponseData = JsonUtility.FromJson<ApiValidateResponse>(apiResponseJson);

        userId = apiResponseData.user_id;
        channelName = apiResponseData.login;

        oauthTokenRetreived = true;
    }

    private async Task RefreshToken()
    {
        string apiUrl;
        string apiResponseJson;
        ApiCodeTokenResponse apiResponseData;

        // construct full URL for API call
        apiUrl = twitchRefreshTokenUrl +
                 "?client_id=" + Secrets.CLIENT_ID +
                 "&client_secret=" + Secrets.CLIENT_SECRET +
                 "&grant_type=refresh_token" +
                 "&refresh_token=" + UnityWebRequest.EscapeURL(_refreshToken);
        
        // make the call!
        apiResponseJson = await CallApi(apiUrl, "POST");

        // parse the return JSON into a more usable data object
        apiResponseData = JsonUtility.FromJson<ApiCodeTokenResponse>(apiResponseJson);

        // fetch the token from the response data
        _authToken = apiResponseData.access_token;
        _refreshToken = apiResponseData.refresh_token;
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

    private async Task setVIP(string targetUserId, bool state)
    {
        string apiUrl;
        apiUrl = twitchVipUrl +
                 "?user_id=" + targetUserId +
                 "&broadcaster_id=" + userId;

        await CallApi(apiUrl, state ? "POST" : "DELETE");
    }
    
    private async Task timeout(string targetUserId, int failedNumber)
    {
        string apiUrl;
        string apiResponseJson;

        apiUrl = twitchBanUrl +
                 "?broadcaster_id=" + userId +
                 "&moderator_id=" + userId;
        
        string body = $"{{\"data\": {{\"user_id\":\"{targetUserId}\",\"duration\":{failedNumber*timeoutMultiplier}}}}}";

        apiResponseJson = await CallApi(apiUrl, "POST", body);
        if(apiResponseJson.Contains("401"))
        {
            await RefreshToken();
            Timeout(targetUserId, failedNumber);
        }
    }

    private async Task<string> CallApi(string endpoint, string method = "GET", string body = "", string[] headers = null)
    {
        _httpClient.BaseAddress = null;
        _httpClient.DefaultRequestHeaders.Clear();

        HttpMethod httpMethod = new HttpMethod(method.ToUpperInvariant());
        HttpRequestMessage httpRequest = new HttpRequestMessage(httpMethod, endpoint);

        if (!string.IsNullOrEmpty(body))
        {
            httpRequest.Content = new StringContent(body, Encoding.UTF8, "application/json");
        }

        if (!string.IsNullOrEmpty(_authToken))
        {
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
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

        HttpResponseMessage httpResponse;
        try
        {
            httpResponse = await _httpClient.SendAsync(httpRequest).ConfigureAwait(false);
            httpResponse.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            // Handle exception or rethrow, depending on your requirement
            throw;
        }

        string httpResponseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        return httpResponseContent;
    }

    private void OnApplicationQuit()
    {
        httpListener?.Stop();
        httpListener?.Abort();
    }


}