using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZavodServer.Models;
using Models;

namespace ZavodServer.Controllers
{
    public class UnitController : BaseController
    {
        
        /// <summary>
        ///     UnitController constructor, that assign database context
        /// </summary>
        /// <param name="db">database context</param>
        public UnitController(DatabaseContext db) : base(db)
        {
        }

        /// <summary>
        ///     Returns all units
        /// </summary>
        /// <returns>All units, or empty list when no units</returns>
        /// <response code="200">Returns all units</response>
        [HttpGet]
        public ActionResult<IEnumerable<UnitDb>> GetAll()
        {
            IEnumerable<UnitDb> result = Db.Units.Select(x => x);
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
            IEnumerable<DefaultUnitDb> result = Db.DefaultUnits.Select(x => x);
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
            if (!Db.Units.Select(x => x.Id).Contains(id))
                return NotFound(id);
            return Ok(Db.Units.First(x => x.Id == id));
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
            if (!Db.Units.Select(x => x.Id).Contains(firstUnitId) || !Db.Units.Select(x => x.Id).Contains(secondUnitId))
                return NotFound("Объекты не найдены");
            return Ok(Vector3.Distance(Db.Units.First(x => x.Id == firstUnitId).Position,
                Db.Units.First(x => x.Id == secondUnitId).Position));
        }

        /// <summary>
        ///     Create unit and save in db
        /// </summary>
        /// <returns>Created unit</returns>
        /// <response code="200">Returns created unit</response>
        [HttpPost]
        public ActionResult<UnitDb> CreateUnit([FromBody] CreateUnitDto createUnit)
        {
            if (!Db.DefaultUnits.Select(x => x.Type).Contains(createUnit.UnitType))
                return NotFound(createUnit.UnitType);
            var unitDto = Db.DefaultUnits.FirstOrDefault(x => x.Type == createUnit.UnitType)?.UnitDto;
            if (unitDto == null)
                return NotFound("No units that type");
            unitDto.Id = Guid.NewGuid();
            unitDto.Position = new Vector3(createUnit.Position.X, createUnit.Position.Y, createUnit.Position.Z);
            Db.Units.Add(unitDto);
            User.Units.Add(unitDto.Id);
            Db.SaveChanges();
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
            if (!HttpContext.Items.TryGetValue("email", out var emailObj))
                return BadRequest();
            var email = emailObj.ToString();
            if (!Db.Users.Any(x => x.Email == email))
                return Unauthorized();
            var userDb = Db.Users.First(x => x.Email == email);
            if (!userDb.Units.Contains(unitDto.Id))
                return BadRequest();
            if (!Db.Units.Select(x => x.Id).Contains(unitDto.Id))
                return NotFound(unitDto);
            var updatingUnit = Db.Units.First(x => x.Id == unitDto.Id);
            Db.Units.Update(updatingUnit);
            updatingUnit.Copy(unitDto);
            Db.SaveChanges();
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
            if (!HttpContext.Items.TryGetValue("email", out var emailObj))
                return BadRequest();
            var email = emailObj.ToString();
            if (!Db.Users.Any(x => x.Email == email))
                return Unauthorized();
            var userDb = Db.Users.First(x => x.Email == email);
            if (!Db.Units.Select(x => x.Id).Contains(id))
                return NotFound(id);
            if (!userDb.Units.Contains(id))
                return BadRequest();
            userDb.Units.Remove(id);
            Db.Units.Remove(Db.Units.First(x => x.Id == id));
            Db.SaveChanges();
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
            if (!HttpContext.Items.TryGetValue("email", out var emailObj))
                return BadRequest();
            var email = emailObj.ToString();
            if (!Db.Users.Any(x => x.Email == email))
                return Unauthorized();
            var userUnitIds = Db.Users.First(x => x.Email == email).Units;
            var userUnits = Db.Units.Where(x =>
                x.CurrentHp > 0 && userUnitIds.Contains(x.Id) && x.SessionId == Session.Id);
            var defenceUnitsIds = unitAttacks.Select(x => x.DefenceUnitId);
            var defenceUnits = Db.Units.Where(x =>
                defenceUnitsIds.Contains(x.Id) && x.CurrentHp > 0 && x.SessionId == Session.Id);
            List<ResultOfAttackDto> attackResult = new List<ResultOfAttackDto>();
            foreach (var unitAttack in unitAttacks)
            {
                var attack = userUnits.FirstOrDefault(x => x.Id.Equals(unitAttack.AttackUnitId));
                var defence = defenceUnits.FirstOrDefault(x => x.Id.Equals(unitAttack.DefenceUnitId));
                if (attack == null || defence == null)
                    continue;
                if (Vector3.Distance(attack.Position, defence.Position) > attack.AttackRange)
                {
                    attackResult.Add(new ResultOfAttackDto {Id = defence.Id, Flag = false, Hp = defence.CurrentHp});
                    continue;
                }

                Db.Units.Update(defence);
                defence.CurrentHp -= attack.AttackDamage;
                attackResult.Add(new ResultOfAttackDto {Id = defence.Id, Flag = true, Hp = defence.CurrentHp});
                Db.SaveChanges();
            }

            return Ok(attackResult);
        }

        /// <summary>
        ///     Move unit to new position
        /// </summary>
        /// <param name="moveUnits"></param>
        /// <returns></returns>
        [HttpPost("move")]
        public async Task<ActionResult<IEnumerable<MoveUnitDto>>> MoveUnit([FromBody] params MoveUnitDto[] moveUnits)
        {
            if (!HttpContext.Items.TryGetValue("email", out var emailObj))
                return BadRequest();
            var email = emailObj.ToString();
            if (!Db.Users.Any(x => x.Email == email))
                return Unauthorized();
            var userUnitIds = (await Db.Users.FirstAsync(x => x.Email == email)).Units;
            var userUnits = Db.Units.Where(x => userUnitIds.Contains(x.Id) && x.CurrentHp > 0);
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

                Db.Units.Update(movesUnit);
                movesUnit.Position = newPosition;
                await Db.SaveChangesAsync();
            }

            return Ok(badMoveResult);
        }
    }
}