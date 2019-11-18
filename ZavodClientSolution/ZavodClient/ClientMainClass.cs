using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ZavodClient
{
    
    public class ClientMainClass
    {
        public static HttpClient Client;
        public static string BaseUrl;

        public ClientMainClass(string baseUrl)
        {
            Client = new HttpClient();
            BaseUrl = baseUrl + "/units/";
        }

        public async Task<UnitDto> CreateUnit(UnitDto objectDto)
        {
            var response = await Client.PostAsJsonAsync(
                BaseUrl, objectDto);
            var unitDto = await response.Content.ReadAsAsync<UnitDto>();
            return unitDto;
        }

        static async Task<UnitDto> GetUnitById(Guid id)
        {
            var response = await Client.GetAsync(BaseUrl + id.ToString());
            response.EnsureSuccessStatusCode();
            var objectDto = await response.Content.ReadAsAsync<UnitDto>();
            return objectDto;
        }

        static async Task<List<Guid>> GetUnitsId()
        {
            var response = await Client.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            var objectsDto = await response.Content.ReadAsAsync<List<Guid>>();
            return objectsDto;
        }
        
        static async Task<float> GetDistanceById(Guid firstUnitId, Guid secondUnitId)
        {
            var response = await Client.GetAsync(BaseUrl + firstUnitId.ToString() +
                                                                 '/' + secondUnitId.ToString());
            response.EnsureSuccessStatusCode();
            var objectDto = await response.Content.ReadAsAsync<float>();
            return objectDto;
        }
        
        static async Task<UnitDto> DeleteUnit(Guid id)
        {
            var response = await Client.DeleteAsync(BaseUrl + id);
            response.EnsureSuccessStatusCode();
            var objectDto = await response.Content.ReadAsAsync<UnitDto>();
            return objectDto;
        }
        
        static async Task<UnitDto> UpdateUnit(UnitDto unitDto)
        {
            var response = await Client.PutAsJsonAsync(BaseUrl, unitDto);
            response.EnsureSuccessStatusCode();
            var updateUnitDto = await response.Content.ReadAsAsync<UnitDto>();
            return updateUnitDto;
        }
    }
}
