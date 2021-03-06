using System;
using System.IO;
using System.Net.Http;
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
                await GetNewAccessToken(ReadRefreshToken());
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
            if (!File.Exists(Path)) return null;            
            var token = File.ReadAllText(Path);
            return JsonConvert.DeserializeObject<SavedRefreshToken>(token).refreshToken;;
        }
        
        public async Task<AccessTokenDto> GetNewAccessToken(string refreshToken)
        {
            var response = await client.PostAsJsonAsync($"{userUrl}refreshToken", refreshToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<AccessTokenDto>();
            if(client.DefaultRequestHeaders.TryGetValues("token", out _))
                client.DefaultRequestHeaders.Remove("token");
            client.DefaultRequestHeaders.Add("token", result.access_token);
            return result;
        }
        private class SavedRefreshToken
        {
            public string refreshToken { get; set; }
        }
    }
}