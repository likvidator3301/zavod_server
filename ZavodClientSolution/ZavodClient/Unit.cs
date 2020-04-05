using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Models;
using NUnit.Framework;

namespace ZavodClient
{
    public class Unit
    {
        
        private static HttpClient client;
        private static string unitUrl;
        private static List<AttackUnitDto> attackUnitsDto = new List<AttackUnitDto>();

        public Unit(string baseUrl)
        {
            client = ZavodClient.Client;
            unitUrl = baseUrl + "/unit/";
        }

        public async Task<List<OutputUnitState>> GetAllUnitStates()
        {
            var response = await client.GetAsync(unitUrl);
            response.EnsureSuccessStatusCode();
            var objectsDto = await response.Content.ReadAsAsync<List<OutputUnitState>>();
            return objectsDto;
        }
        
        public async Task<HttpStatusCode> DestroyUnit(Guid id)
        {
            var response = await client.PostAsJsonAsync($"{unitUrl}destroy/", id);
            response.EnsureSuccessStatusCode();
            return HttpStatusCode.OK;
        }
        
        public async Task<HttpStatusCode> SendUnitsState(params InputUnitState[] unitsDto)
        {
            var response = await client.PostAsJsonAsync(unitUrl, unitsDto);
            response.EnsureSuccessStatusCode();
            return HttpStatusCode.OK;
        }
        
        public async Task<ResultOfAttackDto> AttackUnit(Guid attackUnit, Guid defenceUnit, int damage)
        {
            var attackUnitDto = new AttackUnitDto
            {
                AttackUnitId = attackUnit,
                DefenceUnitId = defenceUnit,
                Damage = damage
            };
            var response = await client.PostAsJsonAsync($"{unitUrl}attack/", new[] {attackUnitDto});
            response.EnsureSuccessStatusCode();
            var updateAttackUnitDto = await response.Content.ReadAsAsync<List<ResultOfAttackDto>>();
            if(updateAttackUnitDto.Count > 0)
                return updateAttackUnitDto[0];
            return null;
        }
        
        public void AddUnitsToAttack(Guid attackUnit, Guid defenceUnit)
        {
            attackUnitsDto.Add(new AttackUnitDto()
            {
                AttackUnitId = attackUnit, 
                DefenceUnitId = defenceUnit
            });
        }

        public async Task<List<ResultOfAttackDto>> SendAttackUnits()
        {
            var response = await client.PostAsJsonAsync($"{unitUrl}attack/", attackUnitsDto);
            response.EnsureSuccessStatusCode();
            var updateAttackUnitsDto = await response.Content.ReadAsAsync<List<ResultOfAttackDto>>();
            return updateAttackUnitsDto;
        }

        public async Task<OutputUnitState> GetUnitById(Guid id)
        {
            var response = await client.GetAsync($"{unitUrl}{id.ToString()}");
            response.EnsureSuccessStatusCode();
            var objectDto = await response.Content.ReadAsAsync<OutputUnitState>();
            return objectDto;
        }
        
        public async Task<float> GetDistanceById(Guid firstUnitId, Guid secondUnitId)
        {
            var response = await client.GetAsync($"{unitUrl}{firstUnitId.ToString()}/{secondUnitId.ToString()}");
            response.EnsureSuccessStatusCode();
            var objectDto = await response.Content.ReadAsAsync<float>();
            return objectDto;
        }
    }
}