using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ZavodServer.Models;

namespace ZavodServer.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("units")]
    public class UnitController : ControllerBase
    {
        private DatabaseContext db = new DatabaseContext();
        /// <summary>
        /// Returns all units id
        /// </summary>
        /// <returns>All units id</returns>
        /// <response code="200">Returns all units id</response>
        /// <response code="404">If no units in db</response> 
        [HttpGet]
        public ActionResult<IEnumerable<UnitDto>> GetUnitsId()
        {
            IEnumerable<UnitDto> result = db.Units.Select(x => x);
            return new ActionResult<IEnumerable<UnitDto>>(result);
        }
        
        /// <summary>
        /// Returns unit by id
        /// </summary>
        /// <returns>Found unit</returns>
        /// <response code="200">Returns unit with id</response>
        /// <response code="404">If no unit in db</response> 
        [HttpGet("{id}")]
        public ActionResult<UnitDto> GetUnitById([FromRoute] Guid id)
        {
            if(!db.Units.Select(x => x.Id).Contains(id))
                return NotFound(id);
            return db.Units.First(x => x.Id == id);
        }

        /// <summary>
        /// Returns distance between {firstUnitId}/{secondUnitId}
        /// </summary>
        /// <returns>Distance between objects</returns>
        /// <response code="200">Returns the distance between units</response>
        /// <response code="404">If unit not found in db</response> 
        [HttpGet("{firstUnitId}/{secondUnitId}")]
        public ActionResult<float> GetDistanceById([FromRoute] Guid firstUnitId, Guid secondUnitId)
        {
            if (!db.Units.Select(x => x.Id).Contains(firstUnitId) || !db.Units.Select(x => x.Id).Contains(secondUnitId))
                return NotFound("Объекты не найдены");
            return Vector3.Distance(db.Units.First(x => x.Id == firstUnitId).Position,
                db.Units.First(x => x.Id == secondUnitId).Position);
        }
        
        /// <summary>
        /// Create unit and save in db
        /// </summary>
        /// <returns>Created unit</returns>
        /// <response code="200">Returns created unit</response> 
        [HttpPost]
        public ActionResult<UnitDto> CreateUnit([FromBody] UnitDto unitDto)
        {
            db.Units.Add(unitDto);
            db.SaveChanges();
            return unitDto;
        }
        
        /// <summary>
        /// Update unit in db
        /// </summary>
        /// <returns>Updated unit</returns>
        /// <response code="200">Returns updated unit</response> 
        /// <response code="404">If unit not found in db</response> 
        [HttpPut]
        public ActionResult<UnitDto> UpdateUnit([FromBody] UnitDto unitDto)
        {
            if (!db.Units.Select(x => x.Id).Contains(unitDto.Id))
                return NotFound(unitDto);
            var updatingUnit = db.Units.First(x => x.Id == unitDto.Id);
            db.Units.Update(updatingUnit);
            updatingUnit.Copy(unitDto);
            db.SaveChanges();
            return updatingUnit;
        }
        
        /// <summary>
        /// Delete unit from db
        /// </summary>
        /// <returns>Deleted unit</returns>
        /// <response code="200">Returns deleted unit</response> 
        /// <response code="404">If unit not found in db</response> 
        [HttpDelete]
        public ActionResult<UnitDto> DeleteUnit([FromBody] UnitDto unitDto)
        {
            if (!db.Units.Select(x => x.Id).Contains(unitDto.Id))
                return NotFound(unitDto);
            db.Units.Remove(unitDto);
            db.SaveChanges();
            return Ok();
        }
    }
}