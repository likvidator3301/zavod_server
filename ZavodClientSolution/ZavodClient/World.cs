using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Models;

namespace ZavodClient
{
    public class World
    {
        private static HttpClient client;
        private static string worldUrl;

        public World(string baseUrl)
        {
            client = ZavodClient.Client;
            worldUrl = baseUrl + "/World/";
        }
        
        public async Task<List<ServerUserDto>> GetWorldState()
        {
            var response = await client.GetAsync(worldUrl);
            response.EnsureSuccessStatusCode();
            var objectServerUserDtos = await response.Content.ReadAsAsync<List<ServerUserDto>>();
            return objectServerUserDtos;
        }
    }
}