using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using ZavodServer.Models;

namespace ZavodServer.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("buildings")]
    public class BuildingController : BaseController
    {
        private readonly DatabaseContext db;

        /// <summary>
        ///     BuildingController constructor, that assign database context
        /// </summary>
        /// <param name="db">database context</param>
        public BuildingController(DatabaseContext db)
        {
            this.db = db;
        }
        
        /// <summary>
        ///     Создание здания
        /// </summary>
        /// <param name="building">
        /// Объект создаваемого здания: его тип и позиция</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<BuildingDb> CreateBuilding([FromBody] CreateBuildingDto building)
        {
            if(!HttpContext.Items.TryGetValue("email", out var emailObj))
                return BadRequest();
            var email = emailObj.ToString();
            if (!db.Users.Any(x => x.Email == email))
                return Unauthorized();
            var userDb = db.Users.First(x => x.Email == email);
            if (!db.DefaultBuildings.Select(x => x.Type).Contains(building.BuildingType))
                return NotFound(building.BuildingType);
            var buildingDto = db.DefaultBuildings.First(x => x.Type == building.BuildingType).BuildingDto;
            buildingDto.Id = Guid.NewGuid();
            buildingDto.Position = building.Position;
            db.Buildings.Add(buildingDto);
            userDb.Buildings.Add(buildingDto.Id);
            db.SaveChanges();
            return buildingDto;
        }
        
        /// <summary>
        ///     Удаление здания
        /// </summary>
        /// <param name="id">
        /// Гуид здания</param>
        /// <returns></returns>
        [HttpDelete]
        public ActionResult<BuildingDb> DeleteBuilding([FromRoute] Guid id)
        {
            if(!HttpContext.Items.TryGetValue("email", out var emailObj))
                return BadRequest();
            var email = emailObj.ToString();
            var userDb = db.Users.First(x => x.Email == email);
            if (!userDb.Buildings.Contains(id))
                return BadRequest();
            if (!db.Buildings.Select(x => x.Id).Contains(id))
                return NotFound(id);
            db.Buildings.Remove(db.Buildings.First(x => x.Id == id));
            db.SaveChanges();
            return Ok();
        }
    }
}