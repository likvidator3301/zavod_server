using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZavodServer.Models;
using Models;

namespace ZavodServer.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("units")]
    public class UnitController : BaseController
    {
        private readonly DatabaseContext db;
        /// <summary>
        ///     UnitController constructor, that assign database context
        /// </summary>
        /// <param name="db">database context</param>
        public UnitController(DatabaseContext db)
        {
            this.db = db;
        }

        /// <summary>
        ///     Returns all units
        /// </summary>
        /// <returns>All units, or empty list when no units</returns>
        /// <response code="200">Returns all units</response>
        [HttpGet]
        public ActionResult<IEnumerable<UnitDb>> GetAll()
        {
            IEnumerable<UnitDb> result = db.Units.Select(x => x);
            return Ok(result);
        }
        
        /// <summary>
        ///     Returns all default units, like templates
        /// </summary>
        /// <returns>All default units, or empty list when no default units</returns>
        /// <response code="200">Returns all default units</response>
        [HttpGet("default")]
        public ActionResult<IEnumerable<DefaultUnitDb>> GetAllDefaultUnits()
        {
            IEnumerable<DefaultUnitDb> result = db.DefaultUnits.Select(x => x);
            return Ok(result);
        }

        /// <summary>
        ///     Returns unit by id
        /// </summary>
        /// <returns>Found unit</returns>
        /// <response code="200">Returns unit with id</response>
        /// <response code="404">If no unit in db</response>
        [HttpGet("{id}")]
        public ActionResult<UnitDb> GetUnitById([FromRoute] Guid id)
        {
            if (!db.Units.Select(x => x.Id).Contains(id))
                return NotFound(id);
            return Ok(db.Units.First(x => x.Id == id));
        }

        /// <summary>
        ///     Returns distance between {firstUnitId}/{secondUnitId}
        /// </summary>
        /// <returns>Distance between objects</returns>
        /// <response code="200">Returns the distance between units</response>
        /// <response code="404">If unit not found in db</response>
        [HttpGet("{firstUnitId}/{secondUnitId}")]
        public ActionResult<float> GetDistanceById([FromRoute] Guid firstUnitId, Guid secondUnitId)
        {
            if (!db.Units.Select(x => x.Id).Contains(firstUnitId) || !db.Units.Select(x => x.Id).Contains(secondUnitId))
                return NotFound("Объекты не найдены");
            return Ok(Vector3.Distance(db.Units.First(x => x.Id == firstUnitId).Position,
                db.Units.First(x => x.Id == secondUnitId).Position));
        }

        /// <summary>
        ///     Create unit and save in db
        /// </summary>
        /// <returns>Created unit</returns>
        /// <response code="200">Returns created unit</response>
        [HttpPost]
        public ActionResult<UnitDb> CreateUnit([FromBody] CreateUnitDto createUnit)
        {
            if (!db.DefaultUnits.Select(x => x.Type).Contains(createUnit.UnitType))
                return NotFound(createUnit.UnitType);
            if(!HttpContext.Items.TryGetValue("email", out var emailObj))
                return BadRequest();
            var email = emailObj.ToString();
            if (!db.Users.Any(x => x.Email == email))
                return Unauthorized();
            var unitDto = db.DefaultUnits.FirstOrDefault(x => x.Type == createUnit.UnitType)?.UnitDto;
            if (unitDto == null)
                return NotFound("No units that type");
            unitDto.Id = Guid.NewGuid();
            unitDto.Position = new Vector3(createUnit.Position.X, createUnit.Position.Y, createUnit.Position.Z);
            db.Units.Add(unitDto);
            db.Users.First(x => x.Email == email).Units.Add(unitDto.Id);
            db.SaveChanges();
            return Ok(unitDto);
        }

        /// <summary>
        ///     Update unit in db
        /// </summary>
        /// <returns>Updated unit</returns>
        /// <response code="200">Returns updated unit</response>
        /// <response code="404">If unit not found in db</response>
        [HttpPut]
        public ActionResult<UnitDb> UpdateUnit([FromBody] UnitDb unitDto)
        {
            if(!HttpContext.Items.TryGetValue("email", out var emailObj))
                return BadRequest();
            var email = emailObj.ToString();
            if (!db.Users.Any(x => x.Email == email))
                return Unauthorized();
            var userDb = db.Users.First(x => x.Email == email);
            if (!userDb.Units.Contains(unitDto.Id))
                return BadRequest();
            if (!db.Units.Select(x => x.Id).Contains(unitDto.Id))
                return NotFound(unitDto);
            var updatingUnit = db.Units.First(x => x.Id == unitDto.Id);
            db.Units.Update(updatingUnit);
            updatingUnit.Copy(unitDto);
            db.SaveChanges();
            return Ok(updatingUnit);
        }

        /// <summary>
        ///     Delete unit from db
        /// </summary>
        /// <returns>Deleted unit</returns>
        /// <response code="204">Returns deleted unit</response>
        /// <response code="404">If unit not found in db</response>
        [HttpDelete("{id}")]
        public ActionResult<Guid> DeleteUnit([FromRoute] Guid id)
        {
            if(!HttpContext.Items.TryGetValue("email", out var emailObj))
                return BadRequest();
            var email = emailObj.ToString();
            if (!db.Users.Any(x => x.Email == email))
                return Unauthorized();
            var userDb = db.Users.First(x => x.Email == email);
            if (!db.Units.Select(x => x.Id).Contains(id))
                return NotFound(id);
            if (!userDb.Units.Contains(id))
                return BadRequest();
            userDb.Units.Remove(id);
            db.Units.Remove(db.Units.First(x => x.Id == id));
            db.SaveChanges();
            return NoContent();
        }

        /// <summary>
        ///     Attack unit
        /// </summary>
        /// <param name="unitAttacks">
        ///    Массив пар id атакующего и атакуемого
        /// </param>
        /// <returns></returns>
        [HttpPost("attack")]
        public ActionResult<IEnumerable<ResultOfAttackDto>> AttackUnit([FromBody] params AttackUnitDto[] unitAttacks)
        {
            if(!HttpContext.Items.TryGetValue("email", out var emailObj))
                return BadRequest();
            var email = emailObj.ToString();
            if (!db.Users.Any(x => x.Email == email))
                return Unauthorized();
            var userUnitIds = db.Users.First(x => x.Email == email).Units;
            var userUnits = db.Units.Where(x => x.CurrentHp > 0 && userUnitIds.Contains(x.Id));
            var defenceUnitsIds = unitAttacks.Select(x => x.DefenceUnitId);
            var defenceUnits = db.Units.Where(x => defenceUnitsIds.Contains(x.Id) && x.CurrentHp > 0);
            List<ResultOfAttackDto> attackResult = new List<ResultOfAttackDto>();
            foreach (var unitAttack in unitAttacks)
            {
                var attack = userUnits.FirstOrDefault(x => x.Id.Equals(unitAttack.AttackUnitId));
                var defence = defenceUnits.FirstOrDefault(x => x.Id.Equals(unitAttack.DefenceUnitId));
                if(attack == null || defence == null)
                    continue;
                if (Vector3.Distance(attack.Position, defence.Position) > attack.AttackRange)
                {
                    attackResult.Add(new ResultOfAttackDto{Id = defence.Id, Flag = false, Hp = defence.CurrentHp});
                    continue;
                }
                db.Units.Update(defence);
                defence.CurrentHp -= attack.AttackDamage;
                attackResult.Add(new ResultOfAttackDto{Id = defence.Id, Flag = true, Hp = defence.CurrentHp});
                db.SaveChanges();
            }
            return Ok(attackResult);
        }

        /// <summary>
        ///     Move unit to new position
        /// </summary>
        /// <param name="moveUnits"></param>
        /// <returns></returns>
        [HttpPost("move")]
        public ActionResult<IEnumerable<MoveUnitDto>> MoveUnit([FromBody] params MoveUnitDto[] moveUnits)
        {
            if(!HttpContext.Items.TryGetValue("email", out var emailObj))
                return BadRequest();
            var email = emailObj.ToString();
            if (!db.Users.Any(x => x.Email == email))
                return Unauthorized();
            var userUnitIds = db.Users.First(x => x.Email == email).Units;
            var userUnits = db.Units.Where(x => userUnitIds.Contains(x.Id) && x.CurrentHp > 0);
            var badMoveResult = new List<MoveUnitDto>();
            foreach (var movingUnit in moveUnits)
            {
                var movesUnit = userUnits.FirstOrDefault(x => x.Id == movingUnit.Id);
                if (movesUnit == null)
                    continue;
                var oldPosition = movesUnit.Position;
                var newPosition = movingUnit.NewPosition;
                if (Vector3.Distance(movesUnit.Position, newPosition) > movesUnit.MoveSpeed)
                {
                    badMoveResult.Add(new MoveUnitDto {Id = movesUnit.Id, NewPosition = oldPosition});
                    continue;
                }

                db.Units.Update(movesUnit);
                movesUnit.Position = newPosition;
                db.SaveChanges();
            }

            return Ok(badMoveResult);
        }
    }
}