using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ZavodClient
{
    
    public class Units
    {
        private static HttpClient _client;
        public static string UnitsBaseUrl;

        public Units(string baseUrl)
        {
            _client = new HttpClient();
            UnitsBaseUrl = baseUrl + "units/";
        }

        public async Task<UnitDto> CreateObject(UnitDto objectDto)
        {
            var response = await _client.PostAsJsonAsync(
                UnitsBaseUrl, objectDto);
            response.EnsureSuccessStatusCode();
            UnitDto unitDto = await response.Content.ReadAsAsync<UnitDto>();
            return unitDto;
        }
        
        public async Task<UnitDto> GetObjectByIdAsync(Guid id)
        {
            var response = await _client.GetAsync(UnitsBaseUrl + id.ToString());
            response.EnsureSuccessStatusCode();
            UnitDto unitDto = await response.Content.ReadAsAsync<UnitDto>();
            return unitDto;
        }

        public async Task<List<Guid>> GetObjectsIds()
        {
            var response = await _client.GetAsync(UnitsBaseUrl);
            response.EnsureSuccessStatusCode();
            List<Guid> unitsDto = await response.Content.ReadAsAsync<List<Guid>>();
            return unitsDto;
        }
    }
}