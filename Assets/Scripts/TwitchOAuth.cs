using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TwitchChat;
using UnityEngine;
using UnityEngine.Networking;

namespace TwitchAPI
{
    public class TwitchOAuth : MonoBehaviour
    {
        private readonly string twitchValidateUrl = "https://id.twitch.tv/oauth2/validate";
        private readonly string twitchBanUrl = "https://api.twitch.tv/helix/moderation/bans";
        private readonly string twitchAuthUrl = "https://id.twitch.tv/oauth2/authorize";
        private readonly string twitchVipUrl = "https://api.twitch.tv/helix/channels/vips";
        private readonly string twitchSettingsUrl = "https://api.twitch.tv/helix/chat/settings";

        public readonly string twitchRedirectHost = "http://localhost:";
        private int twitchFreePort;

        [SerializeField] bool useUUUID = true;

        public readonly string uselessUUID =
            "53125396-3e32-4fad-8f7e-36475724168b-a8fe83ab-3373-4a6a-8967-2532eafe407f-41483db3-f011-4a23-80da-9a340672692a-e755c6d4-c546-43ce-b722-b5a799561b4e-5ba1697d-79b2-4d5d-96c3-f0d91f13f583-f08f18f9-bd56-4a0f-a597-96f90108cd85-14449d50-6cc9-450f-8119-ff4c525e31db-e41a6912-92a0-48b6-b6d3-845c21bea7eb-7dfd7948-2976-42cf-9cca-b23ae5854813-107224eb-81ea-46dd-9bf5-9ebbfcfc45dc/";

        [SerializeField] private string loginSuccessMessage = "<b>Done!</b><br>You can close this window now!";
        [SerializeField] private string loginFailMessage = "<b>Oops!</b><br>Something bad happened. Please restart de game and try again.";

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

        private int freePort;

        [SerializeField] private string clientId;

        [SerializeField] private int[] portList = new[]
        {
            12345,
            1234,
            12346
        };

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
            UpdateTwitchSettings();
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
            List<string> scopes = new List<string> { "chat:read+moderator:manage:chat_settings" };

            if (enableTimeout)
            {
                scopes.Add("moderator:manage:banned_users");
            }

            if (enableVip)
            {
                scopes.Add("channel:manage:vips");
            }

            if (!CheckAvailablePort())
            {
                Debug.Log("Not available port");
                return;
            }

            // generate something for the "state" parameter.
            // this can be whatever you want it to be, it's gonna be "echoed back" to us as is and should be used to
            // verify the redirect back from Twitch is valid.
            twitchAuthStateVerify = ((Int64)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();

            string redirectUri = useUUUID
                ? twitchRedirectHost + freePort + "/" + uselessUUID
                : twitchRedirectHost + freePort + "/";

            var s = "client_id=" + clientId + "&" +
                    "redirect_uri=" + UnityWebRequest.EscapeURL(redirectUri) +
                    "&" +
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
            
            string redirectUri = useUUUID
                ? twitchRedirectHost + freePort + "/" + uselessUUID
                : twitchRedirectHost + freePort + "/";
            
            httpListener.Prefixes.Add(redirectUri);
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
                    string responseString =
                        $"<html><body>{loginSuccessMessage}</body></html>";
                    ValidateToken(true);
                    SendResponse(httpContext, responseString);

                    httpListener.Stop();
                }
                else
                {
                    string responseString =
                        $"<html><body>{loginFailMessage}</body></html>";
                    SendResponse(httpContext, responseString);

                    httpListener.Stop();
                }
            }
            else if (tokens.Contains("error"))
            {
                string responseString =
                    $"<html><body>{loginFailMessage}</body></html>";
                SendResponse(httpContext, responseString);

                httpListener.Stop();
            }
            else
            {
                string responseString =
                    "<html><head><meta http-equiv='cache-control' content='no-cache'><meta http-equiv='expires' content='0'> <meta http-equiv='pragma' content='no-cache'></head><body><script>var link = window.location.toString(); link = link.replace('#','?'); window.location.replace(link);</script></body></html>";
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

            if (shouldConnectChat) oauthTokenRetrieved = true;
        }

        private async Task UpdateTwitchSettings()
        {
            string apiUrl = twitchSettingsUrl +
                            "?broadcaster_id" + userId +
                            "&moderator_id" + userId;
            string body = $"{{\"data\": {{\"non_moderator_chat_delay\":false,\"unique_chat_mode\":false}}}}";
            await CallApi(apiUrl, "PATCH", body);
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

            string body =
                $"{{\"data\": {{\"user_id\":\"{targetUserId}\",\"duration\":{failedNumber * timeoutMultiplier}}}}}";

            await CallApi(apiUrl, "POST", body);
        }

        private async Task<string> CallApi(string endpoint, string method = "GET", string body = "",
            string[] headers = null)
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

            if (!string.IsNullOrEmpty(clientId))
            {
                httpRequest.Headers.TryAddWithoutValidation("Client-Id", clientId);
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

        private bool CheckAvailablePort()
        {
            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.

            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (int port in portList)
            {
                if (tcpConnInfoArray.All(x => x.LocalEndPoint.Port != port))
                {
                    freePort = port;
                    return true;
                }
            }

            return false;
        }

        private void OnApplicationQuit()
        {
            httpListener?.Stop();
            httpListener?.Abort();
        }
    }
}