using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Models;
using ZavodServer.Models;

namespace ZavodServer.Controllers
{
    [Produces("application/json")]
    [ApiController]
//    [Authorize]
    [Route("buildings")]
    public class BuildingController : ControllerBase
    {
        private readonly DatabaseContext db = new DatabaseContext();

        /// <summary>
        ///     Создание здания
        /// </summary>
        /// <param name="buildingType">
        /// Тип создаваемого здания</param>
        /// <returns></returns>
        public ActionResult<BuildingDb> CreateBuilding([FromBody] BuildingType buildingType)
        {
            if (!db.DefaultBuildings.Select(x => x.Type).Contains(buildingType))
                return NotFound(buildingType);
            var buildingDto = db.DefaultBuildings.First(x => x.Type == buildingType).BuildingDto;
            db.Buildings.Add(buildingDto);
            db.SaveChanges();
            return buildingDto;
        }
        
        /// <summary>
        ///     Удаление ддания
        /// </summary>
        /// <param name="id">
        /// Гуид здания</param>
        /// <returns></returns>
        [HttpDelete]
        public ActionResult<BuildingDb> DeleteBuilding([FromRoute] Guid id)
        {
            if (!db.Buildings.Select(x => x.Id).Contains(id))
                return NotFound(id);
            db.Buildings.Remove(db.Buildings.First(x => x.Id == id));
            db.SaveChanges();
            return Ok();
        }
    }
}