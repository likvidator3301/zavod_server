using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ZavodClient
{
    
    public class ClientMainClass
    {
        private static HttpClient _client;
        private static string _baseUrl;

        public ClientMainClass(string baseUrl)
        {
            _client = new HttpClient();
            _baseUrl = baseUrl + "/units/";
        }

        public async Task<UnitDto> CreateUnit(UnitDto objectDto)
        {
            var response = await _client.PostAsJsonAsync(
                _baseUrl, objectDto);
            var unitDto = await response.Content.ReadAsAsync<UnitDto>();
            return unitDto;
        }

        public async Task<UnitDto> GetUnitById(Guid id)
        {
            var response = await _client.GetAsync($"{_baseUrl}{id.ToString()}");
            response.EnsureSuccessStatusCode();
            var objectDto = await response.Content.ReadAsAsync<UnitDto>();
            return objectDto;
        }

        public async Task<List<Guid>> GetUnitsId()
        {
            var response = await _client.GetAsync(_baseUrl);
            response.EnsureSuccessStatusCode();
            var objectsDto = await response.Content.ReadAsAsync<List<Guid>>();
            return objectsDto;
        }
        
        public async Task<float> GetDistanceById(Guid firstUnitId, Guid secondUnitId)
        {
            var response = await _client.GetAsync($"{_baseUrl}{firstUnitId.ToString()}/{secondUnitId.ToString()}");
            response.EnsureSuccessStatusCode();
            var objectDto = await response.Content.ReadAsAsync<float>();
            return objectDto;
        }
        
        public async Task<HttpStatusCode> DeleteUnit(Guid id)
        {
            var response = await _client.DeleteAsync($"{_baseUrl}{id.ToString()}");
            response.EnsureSuccessStatusCode();
            return HttpStatusCode.OK;
        }
        
        public async Task<UnitDto> UpdateUnit(UnitDto unitDto)
        {
            var response = await _client.PutAsJsonAsync(_baseUrl, unitDto);
            response.EnsureSuccessStatusCode();
            var updateUnitDto = await response.Content.ReadAsAsync<UnitDto>();
            return updateUnitDto;
        }
    }
}
