using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Models;

namespace ZavodClient
{
    public class Bag
    {
        private static HttpClient client;
        private static string bagUrl;

        public Bag(string baseUrl)
        {
            client = ZavodClient.Client;
            bagUrl = baseUrl + "/bag/";
        }

        public async Task<List<BagDto>> GetAll()
        {
            var response = await client.GetAsync(bagUrl);
            response.EnsureSuccessStatusCode();
            var objectDto = await response.Content.ReadAsAsync<List<BagDto>>();
            return objectDto;
        }

        public async Task<HttpStatusCode> Destroy(Guid id)
        {
            var response = await client.DeleteAsync($"{bagUrl}{id.ToString()}");
            response.EnsureSuccessStatusCode();
            return HttpStatusCode.OK;
        }

        public async Task<HttpStatusCode> Create(Guid id, int goldCount, Vector3 position)
        {
            var bag = new BagDto
            {
                Id = id,
                GoldCount = goldCount,
                Position = position
            };
            var response = await client.PostAsJsonAsync(
                bagUrl, bag);
            response.EnsureSuccessStatusCode();
            return HttpStatusCode.OK;
        }
    }
}