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

        public async Task<List<ServerUnitDto>> GetAllUnitStates()
        {
            var response = await client.GetAsync(unitUrl);
            response.EnsureSuccessStatusCode();
            var objectsDto = await response.Content.ReadAsAsync<List<ServerUnitDto>>();
            return objectsDto;
        }
        
        public async Task<HttpStatusCode> DestroyUnit(Guid id)
        {
            var response = await client.DeleteAsync($"{unitUrl}{id.ToString()}");
            response.EnsureSuccessStatusCode();
            return HttpStatusCode.OK;
        }
        
        public async Task<List<ServerUnitDto>> SendUnitsState(params ServerUnitDto[] unitsDto)
        {
            var response = await client.PutAsJsonAsync(unitUrl, unitsDto);
            response.EnsureSuccessStatusCode();
            var updateObjectsDto = await response.Content.ReadAsAsync<List<ServerUnitDto>>();
            return updateObjectsDto;
        }
        
        public async Task<ResultOfAttackDto> AttackUnit(Guid attackUnit, Guid defenceUnit, int damage = 0)
        {
            var attackUnitDto = new AttackUnitDto
            {
                AttackUnitId = attackUnit,
                DefenceUnitId = defenceUnit,
                Damage = damage
            };
            var response = await client.PutAsJsonAsync($"{unitUrl}attack/", attackUnitDto);
            response.EnsureSuccessStatusCode();
            var updateAttackUnitDto = await response.Content.ReadAsAsync<List<ResultOfAttackDto>>();
            return updateAttackUnitDto[0];
        }
        
        public void AddUnitsToAttack(Guid attackUnit, Guid defenceUnit)
        {
            attackUnitsDto.Add(new AttackUnitDto()
            {
                AttackUnitId = attackUnit, 
                DefenceUnitId = defenceUnit
            });
        }

        public async Task<List<Guid>> SendAttackUnits()
        {
            var response = await client.PutAsJsonAsync($"{unitUrl}attack/", attackUnitsDto);
            response.EnsureSuccessStatusCode();
            var updateAttackUnitsDto = await response.Content.ReadAsAsync<List<Guid>>();
            return updateAttackUnitsDto;
        }

        public async Task<ServerUnitDto> GetUnitById(Guid id)
        {
            var response = await client.GetAsync($"{unitUrl}{id.ToString()}");
            response.EnsureSuccessStatusCode();
            var objectDto = await response.Content.ReadAsAsync<ServerUnitDto>();
            return objectDto;
        }
        
        public async Task<float> GetDistanceById(Guid firstUnitId, Guid secondUnitId)
        {
            var response = await client.GetAsync($"{unitUrl}{firstUnitId.ToString()}/{secondUnitId.ToString()}");
            response.EnsureSuccessStatusCode();
            var objectDto = await response.Content.ReadAsAsync<float>();
            return objectDto;
        }
        
        public async Task<ServerUnitDto> CreateUnit(CreateUnitDto createUnitDto)
        {
            var response = await client.PostAsJsonAsync(
                unitUrl, createUnitDto);
            var objectDto = await response.Content.ReadAsAsync<ServerUnitDto>();
            return objectDto;
        }
    }
}