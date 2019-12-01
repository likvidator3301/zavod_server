using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Models;

namespace ZavodClient
{
    public class Building
    {
        private static HttpClient client;
        private static string unitUrl;

        public Building(string baseUrl)
        {
            client = ZavodClient.Client;
            unitUrl = baseUrl + "/buildings/";
        }

        public async Task<ServerBuildingDto> CreateBuilding(BuildingType buildingType)
        {
            var response = await client.PostAsJsonAsync(
                unitUrl, buildingType);
            var buildingDto = await response.Content.ReadAsAsync<ServerBuildingDto>();
            return buildingDto;
        }

        public async Task<HttpStatusCode> DeleteBuilding(Guid id)
        {
            var response = await client.DeleteAsync($"{unitUrl}{id.ToString()}");
            response.EnsureSuccessStatusCode();
            return HttpStatusCode.OK;
        }
    }
}