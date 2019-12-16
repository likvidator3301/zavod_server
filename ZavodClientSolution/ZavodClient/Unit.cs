using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Models;

namespace ZavodClient
{
    public class Unit
    {
        
        private static HttpClient client;
        private static string unitUrl;

        private static List<AttackUnitDto> attackUnitsDto;
        private static List<MoveUnitDto> moveUnitsDto;

        public Unit(string baseUrl)
        {
            client = ZavodClient.Client;
            unitUrl = baseUrl + "/units/";
        }

        public void AddUnitsToAttack(Guid attackUnit, Guid defenceUnit)
        {
            attackUnitsDto.Add(new AttackUnitDto()
            {
                Attack = attackUnit, 
                Defence = defenceUnit
            });
        }
        
        public void AddUnitsToMove(Guid id, Vector3 newPosition)
        {
            moveUnitsDto.Add(new MoveUnitDto()
            {
                Id = id,
                NewPosition = newPosition
            });
        }
        
        public async Task<List<ServerUnitDto>> GetAll()
        {
            var response = await client.GetAsync(unitUrl);
            response.EnsureSuccessStatusCode();
            var objectsDto = await response.Content.ReadAsAsync<List<ServerUnitDto>>();
            return objectsDto;
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
        
        public async Task<ServerUnitDto> UpdateUnit(ServerUnitDto unitDto)
        {
            var response = await client.PutAsJsonAsync(unitUrl, unitDto);
            response.EnsureSuccessStatusCode();
            var updateObjectDto = await response.Content.ReadAsAsync<ServerUnitDto>();
            return updateObjectDto;
        }
        
        public async Task<HttpStatusCode> DeleteUnit(Guid id)
        {
            var response = await client.DeleteAsync($"{unitUrl}{id.ToString()}");
            response.EnsureSuccessStatusCode();
            return HttpStatusCode.OK;
        }

        public async Task<List<ResultOfAttackDto>> SendAttackUnits()
        {
            var response = await client.PutAsJsonAsync($"{unitUrl}attack/", attackUnitsDto);
            response.EnsureSuccessStatusCode();
            var updateAttackUnitsDto = await response.Content.ReadAsAsync<List<ResultOfAttackDto>>();
            return updateAttackUnitsDto;
        }

        public async Task<List<MoveUnitDto>> SendMoveUnits()
        {
            var response = await client.PutAsJsonAsync($"{unitUrl}move/", moveUnitsDto);
            response.EnsureSuccessStatusCode();
            var updateMoveUnitsDto = await response.Content.ReadAsAsync<List<MoveUnitDto>>();
            return updateMoveUnitsDto;
        }
    }
}