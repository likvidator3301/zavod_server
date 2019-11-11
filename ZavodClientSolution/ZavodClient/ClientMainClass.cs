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

        public ClientMainClass()
        {
            Client = new HttpClient();
            BaseUrl = Config.BaseUrlUnits;
        }

        public async Task<Uri> CreateObject(UnitDto objectDto)
        {
            HttpResponseMessage response = await Client.PostAsJsonAsync(
                BaseUrl, objectDto);
            response.EnsureSuccessStatusCode();
            return response.Headers.Location;
        }

        static async Task<UnitDto> GetObjectByIdAsync(Guid id)
        {
            HttpResponseMessage response = await Client.GetAsync(BaseUrl + id.ToString());
            response.EnsureSuccessStatusCode();
            UnitDto objectDto = await response.Content.ReadAsAsync<UnitDto>();
            return objectDto;
        }

        static async Task<List<Guid>> GetObjectsIds()
        {
            HttpResponseMessage response = await Client.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            List<Guid> objectsDto = await response.Content.ReadAsAsync<List<Guid>>();
            return objectsDto;
        }
    }
}
