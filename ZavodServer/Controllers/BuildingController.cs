using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        ///     Возвращает все здания в данной сессии
        /// </summary>
        /// <returns>
        ///    Список всех зданий в сессиии
        /// </returns>
        [HttpGet]
        public ActionResult<IEnumerable<BuildingDb>> GetAllBuildings()
        {
            if (Session == null)
                return BadRequest();
            return Ok(Db.Buildings.Where(x => x.SessionId.Equals(Session.Id)));
        }
        
        
        /// <summary>
        ///     Создание здания
        /// </summary>
        /// <param name="building">
        /// Объект создаваемого здания: его тип и позиция</param>
        /// <returns>
        ///    Объект созданного здания
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<BuildingDb>> CreateBuilding([FromBody] CreateBuildingDto building)
        {
            if (Session == null)
                return BadRequest();
            var buildingDto = (await Db.DefaultBuildings.FirstOrDefaultAsync(x => x.Type == building.BuildingType))?.BuildingDto;
            if (buildingDto == null)
                return NotFound(building.BuildingType);
            buildingDto.Id = Guid.NewGuid();
            buildingDto.Position = building.Position;
            buildingDto.OwnerId = UserDb.Id;
            buildingDto.SessionId = Session.Id;
            Db.Buildings.Add(buildingDto);
            return buildingDto;
        }
        
        /// <summary>
        ///     Удаление здания
        /// </summary>
        /// <param name="id">
        /// Гуид здания</param>
        /// <returns>
        ///    Статус удалилось ли здание
        /// </returns>
        [HttpDelete]
        public async Task<ActionResult> DeleteBuilding([FromRoute] Guid id)
        {
            if (Session == null || await Db.Buildings.FirstOrDefaultAsync(x => 
                x.SessionId.Equals(Session.Id) && x.OwnerId.Equals(UserDb.Id) && x.Id.Equals(id)) == null)
                return BadRequest();
            var building = await Db.Buildings.FirstOrDefaultAsync(x => x.Id == id);
            if (building == null)
                return NotFound(id);
            Db.Buildings.Remove(building);
            return Ok();
        }
    }
}