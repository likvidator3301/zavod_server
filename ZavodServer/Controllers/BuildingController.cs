using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using ZavodServer.Models;

namespace ZavodServer.Controllers
{
    public class BuildingController : BaseController
    {
        /// <summary>
        ///     BuildingController constructor, that assign database context
        /// </summary>
        /// <param name="db">database context</param>
        public BuildingController(DatabaseContext db): base(db)
        {
        }
        
        /// <summary>
        ///     Создание здания
        /// </summary>
        /// <param name="building">
        /// Объект создаваемого здания: его тип и позиция</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BuildingDb>> CreateBuilding([FromBody] CreateBuildingDto building)
        {
            var buildingDto = (await Db.DefaultBuildings.FirstOrDefaultAsync(x => x.Type == building.BuildingType))?.BuildingDto;
            if (buildingDto == null)
                return NotFound(building.BuildingType);
            buildingDto.Id = Guid.NewGuid();
            buildingDto.Position = building.Position;
            Db.Buildings.Add(buildingDto);
            UserDb.Buildings.Add(buildingDto.Id);
            await Db.SaveChangesAsync();
            return buildingDto;
        }
        
        /// <summary>
        ///     Удаление здания
        /// </summary>
        /// <param name="id">
        /// Гуид здания</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult<BuildingDb>> DeleteBuilding([FromRoute] Guid id)
        {
            if (!UserDb.Buildings.Contains(id))
                return BadRequest();
            var building = await Db.Buildings.FirstOrDefaultAsync(x => x.Id == id);
            if (building == null)
                return NotFound(id);
            Db.Buildings.Remove(building);
            await Db.SaveChangesAsync();
            return Ok();
        }
    }
}