using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Models;

namespace ZavodClient
{
    
    public class Units
    {
        private static HttpClient _client;
        private static string _baseUrl;

        public Units(string baseUrl)
        {
            _client = new HttpClient();
            _baseUrl = baseUrl + "/units/";
        }

        public async Task<ServerUnitDto> CreateUnit(ServerUnitDto unitDto)
        {
            var response = await _client.PostAsJsonAsync(
                _baseUrl, unitDto);
            var objectDto = await response.Content.ReadAsAsync<ServerUnitDto>();
            return objectDto;
        }

        public async Task<ServerUnitDto> GetUnitById(Guid id)
        {
            var response = await _client.GetAsync($"{_baseUrl}{id.ToString()}");
            response.EnsureSuccessStatusCode();
            var objectDto = await response.Content.ReadAsAsync<ServerUnitDto>();
            return objectDto;
        }

        public async Task<List<ServerUnitDto>> GetAll()
        {
            var response = await _client.GetAsync(_baseUrl);
            response.EnsureSuccessStatusCode();
            var objectsDto = await response.Content.ReadAsAsync<List<ServerUnitDto>>();
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
        
        public async Task<ServerUnitDto> UpdateUnit(ServerUnitDto unitDto)
        {
            var response = await _client.PutAsJsonAsync(_baseUrl, unitDto);
            response.EnsureSuccessStatusCode();
            var updateObjectDto = await response.Content.ReadAsAsync<ServerUnitDto>();
            return updateObjectDto;
        }
    }
}
