using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Models;

namespace ZavodClient
{
    public class Session
    {
        private static HttpClient client;
        private static string sessionUrl;

        public Session(string baseUrl)
        {
            client = ZavodClient.Client;
            sessionUrl = baseUrl + "/session/";
        }

        public async Task<HttpStatusCode> CreateSession(string mapName)
        {
            var response = await client.PostAsJsonAsync(
                sessionUrl, mapName);
            response.EnsureSuccessStatusCode();
            return HttpStatusCode.OK;
        }

        public async Task<SessionDto> GetSession(Guid id)
        {
            var response = await client.GetAsync($"{sessionUrl}{id.ToString()}");
            response.EnsureSuccessStatusCode();
            var objectDto = await response.Content.ReadAsAsync<SessionDto>();
            return objectDto;
        }
        
        public async Task<List<SessionDto>> GetAllSessions()
        {
            var response = await client.GetAsync(sessionUrl);
            response.EnsureSuccessStatusCode();
            var objectDto = await response.Content.ReadAsAsync<List<SessionDto>>();
            return objectDto;
        }
        
        public async Task<HttpStatusCode> EnterSessions(EnterSessionRequest enterSessionRequest)
        {
            var response = await client.PostAsJsonAsync(
                $"{sessionUrl}/enter/", enterSessionRequest);
            response.EnsureSuccessStatusCode();
            return HttpStatusCode.OK;
        }

        public async Task<List<PlayerDto>> GetPlayers()
        {
            var response = await client.GetAsync($"{sessionUrl}/players/");
            response.EnsureSuccessStatusCode();
            var objectDto = await response.Content.ReadAsAsync<List<PlayerDto>>();
            return objectDto;
        }

        public async Task<HttpStatusCode> SendPlayerState(InputPlayerModel inputPlayerModel)
        {
            var response = await client.PostAsJsonAsync(
                $"{sessionUrl}/player/", inputPlayerModel);
            response.EnsureSuccessStatusCode();
            return HttpStatusCode.OK;
        }
    }
}