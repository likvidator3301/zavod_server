using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Timers;
using Models;
using Newtonsoft.Json;

namespace ZavodClient
{
    public class User
    {
        private static HttpClient client;
        private static string userUrl, authUrl;
        private readonly Timer timer;
        private string deviceCode;
        private const string Path = "./token.json";

        public Action<ServerUserDto> OnRegisterSuccessful;
        
        public User(string baseUrl)
        {
            client = ZavodClient.Client;
            userUrl = baseUrl + "/token/";
            authUrl = baseUrl + "/auth/";
            timer = new Timer(5000);
            timer.Elapsed += ( sender, e ) => PollGoogle();
        }

        public bool IsRegistered => ReadRefreshToken() != null;

        public async Task<ServerUserDto> GetUser()
        {
            if (IsRegistered)
            {
                var token = (await GetNewAccessToken(ReadRefreshToken())).access_token;
                client.DefaultRequestHeaders.Add("token", token);
                var response = await client.GetAsync(authUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<ServerUserDto>();
            }
            return null;
        }
        
        public async Task<GoogleAuthDto> GetAuthCode()
        {
            var response = await client.GetAsync(userUrl);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<GoogleAuthDto>();            
            deviceCode = result.device_code;
            timer.Start();
            return result;
        }

        private async void PollGoogle()
        {
            var response = await client.PostAsJsonAsync(userUrl + "pollGoogle",deviceCode);
            if (!response.IsSuccessStatusCode) return;
            timer.Stop();
            var result = await response.Content.ReadAsAsync<PollingResult>();
            File.WriteAllText(Path, JsonConvert.SerializeObject(new SavedRefreshToken{refreshToken = result.Tokens.refresh_token}));
            OnRegisterSuccessful(result.User);
        }

        private string ReadRefreshToken()
        {
            string token = null;
            if (File.Exists(Path))
                token = JsonConvert.DeserializeObject<SavedRefreshToken>(File.ReadAllText(Path)).refreshToken;
            return token;
        }
        
        public async Task<AccessTokenDto> GetNewAccessToken(string refreshToken)
        {
            var response = await client.PostAsJsonAsync(userUrl +"refreshToken", refreshToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<AccessTokenDto>();
            client.DefaultRequestHeaders.Add("token", result.access_token);
            return result;
        }
        private class SavedRefreshToken
        {
            public string refreshToken { get; set; }
        }
    }
}