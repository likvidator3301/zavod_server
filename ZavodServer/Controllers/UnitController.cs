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
        public UnitController(DatabaseContext db) : base(db){}

        /// <summary>
        ///     Returns all units
        /// </summary>
        /// <returns>All units, or empty list when no units</returns>
        /// <response code="200">Returns all units</response>
        [HttpGet]
        public ActionResult<IEnumerable<UnitDb>> GetAll()
        {
            IEnumerable<UnitDb> result = Db.Units.Where(x => x.SessionId.Equals(Session.Id));
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
        public async Task<ActionResult<float>> GetDistanceById([FromRoute] Guid firstUnitId, Guid secondUnitId)
        {
            var firstUnitPosition = (await Db.Units.FirstOrDefaultAsync(x => x.Id == firstUnitId))?.Position;
            var secondUnitPosition = (await Db.Units.FirstOrDefaultAsync(x => x.Id == secondUnitId))?.Position;
            if (firstUnitPosition == null || secondUnitPosition == null )
                return NotFound("Not found units");
            return Ok(Vector3.Distance(firstUnitPosition, secondUnitPosition));
        }

        /// <summary>
        ///     Create unit and save in db
        /// </summary>
        /// <returns>Created unit</returns>
        /// <response code="200">Returns created unit</response>
        [HttpPost]
        public async Task<ActionResult<UnitDb>> CreateUnit([FromBody] CreateUnitDto createUnit)
        {
            if (Session == null)
                return BadRequest();
            var unitDto = (await Db.DefaultUnits.FirstOrDefaultAsync(x => x.Type == createUnit.UnitType))?.UnitDto;
            if (unitDto == null)
                return NotFound(createUnit.UnitType);
            unitDto.Id = Guid.NewGuid();
            unitDto.Position = new Vector3(createUnit.Position.X, createUnit.Position.Y, createUnit.Position.Z);
            unitDto.OwnerId = UserDb.Id;
            unitDto.SessionId = Session.Id;
            Db.Units.Add(unitDto);
            await Db.SaveChangesAsync();
            return Ok(unitDto);
        }

        /// <summary>
        ///     Update unit in db
        /// </summary>
        /// <returns>Updated unit</returns>
        /// <response code="200">Returns updated unit</response>
        /// <response code="404">If unit not found in db</response>
        [HttpPut]
        public async Task<ActionResult<UnitDb>> UpdateUnit([FromBody] UnitDb unitDto)
        {
            if (Session == null)
                return BadRequest();
            if (await Db.Units.FirstOrDefaultAsync(x => 
                x.Id.Equals(unitDto.Id) && x.SessionId.Equals(Session.Id) && x.OwnerId.Equals(UserDb.Id)) == null)
                return BadRequest();
            var updatingUnit = await Db.Units.FirstOrDefaultAsync(x => x.Id == unitDto.Id);
            if (updatingUnit == null)
                return NotFound(unitDto);
            Db.Units.Update(updatingUnit);
            updatingUnit.Copy(unitDto);
            await Db.SaveChangesAsync();
            return Ok(updatingUnit);
        }

        /// <summary>
        ///     Delete unit from db
        /// </summary>
        /// <returns>Deleted unit</returns>
        /// <response code="204">Returns deleted unit</response>
        /// <response code="404">If unit not found in db</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Guid>> DeleteUnit([FromRoute] Guid id)
        {
            if (Session == null)
                return BadRequest();
            var unit = await Db.Units.FirstOrDefaultAsync(x =>
                x.Id.Equals(id) && x.SessionId.Equals(Session.Id) && x.OwnerId.Equals(UserDb.Id));
            if (unit == null)
                return NotFound();
            Db.Units.Remove(unit);
            await Db.SaveChangesAsync();
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
        public async Task<ActionResult<IEnumerable<ResultOfAttackDto>>> AttackUnit([FromBody] params AttackUnitDto[] unitAttacks)
        {
            if (Session == null)
                return BadRequest();
            var userUnits = Db.Units.Where(x =>
                x.Id.Equals(UserDb.Id) && x.CurrentHp > 0 && x.SessionId.Equals(Session.Id) && x.OwnerId.Equals(UserDb.Id));
            var defenceUnitsIds = unitAttacks.Select(x => x.DefenceUnitId);
            var defenceUnits = Db.Units.Where(x =>
                defenceUnitsIds.Contains(x.Id) && x.CurrentHp > 0);
            var attackResult = new List<ResultOfAttackDto>();
            foreach (var unitAttack in unitAttacks)
            {
                var attack = await userUnits.FirstOrDefaultAsync(x => x.Id.Equals(unitAttack.AttackUnitId));
                var defence = await defenceUnits.FirstOrDefaultAsync(x => x.Id.Equals(unitAttack.DefenceUnitId));
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
            }
            
            await Db.SaveChangesAsync();
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
            if (Session == null)
                return BadRequest();
            var userUnits = Db.Units.Where(x =>
                x.Id.Equals(UserDb.Id) && x.CurrentHp > 0 && x.SessionId.Equals(Session.Id) && x.OwnerId.Equals(UserDb.Id));
            var badMoveResult = new List<MoveUnitDto>();
            foreach (var movingUnit in moveUnits)
            {
                var movesUnit =
                    await userUnits.FirstOrDefaultAsync(x =>
                        x.Id.Equals(movingUnit.Id) && x.SessionId.Equals(Session.Id));
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
            }
            
            await Db.SaveChangesAsync();
            return Ok(badMoveResult);
        }
    }
}