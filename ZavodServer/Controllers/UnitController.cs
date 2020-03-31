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
            if (Session == null)
                return BadRequest();
            IEnumerable<UnitDb> result = Db.Units.Where(x => x.SessionId.Equals(Session.Id));
            return Ok(result);
        }
        
        /// <summary>
        ///     Update unit in db
        /// </summary>
        /// <returns>Updated unit</returns>
        /// <response code="200">Returns updated unit</response>
        /// <response code="404">If unit not found in db</response>
        [HttpPut]
        public async Task<ActionResult> UpdateUnit([FromBody] UnitDb unitDto)
        {
            if (Session == null)
                return BadRequest();
            if (await Db.Units.FirstOrDefaultAsync(x =>
                x.Id.Equals(unitDto.Id) && x.SessionId.Equals(Session.Id) && x.PlayerId.Equals(UserDb.Id)) == null)
            {
                unitDto.Health = UnitDb.GetMaxHpFromType(unitDto.Type);
                unitDto.PlayerId = UserDb.MyPlayer.Id;
                Db.Units.Add(unitDto);
                return Ok();
            }
            var updatingUnit = await Db.Units.FirstOrDefaultAsync(x => x.Id == unitDto.Id);
            if (updatingUnit == null)
                return NotFound(unitDto);
            Db.Units.Update(updatingUnit);
            updatingUnit.Copy(unitDto);
            return Ok();
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
        ///     Delete unit from db
        /// </summary>
        /// <returns>Deleted unit</returns>
        /// <response code="204">Returns deleted unit</response>
        /// <response code="404">If unit not found in db</response>
        [HttpPost("{id}")]
        public async Task<ActionResult<Guid>> DestroyUnit([FromRoute] Guid id)
        {
            if (Session == null)
                return BadRequest();
            var unit = await Db.Units.FirstOrDefaultAsync(x =>
                x.Id.Equals(id) && x.SessionId.Equals(Session.Id) && x.PlayerId.Equals(UserDb.MyPlayer.Id));
            if (unit == null)
                return NotFound();
            Db.Units.Update(unit);
            unit.Health = 0;
            return Ok();
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
                x.SessionId.Equals(Session.Id) && x.PlayerId.Equals(UserDb.MyPlayer.Id) && x.Health > 0);
            var defenceUnitsIds = unitAttacks.Select(x => x.DefenceUnitId);
            var defenceUnits = Db.Units.Where(x => 
                defenceUnitsIds.Contains(x.Id) && x.SessionId.Equals(Session.Id) && x.Health > 0);
            var attackResult = new List<ResultOfAttackDto>();
            foreach (var unitAttack in unitAttacks)
            {
                var attack = await userUnits.FirstOrDefaultAsync(x => x.Id.Equals(unitAttack.AttackUnitId));
                var defence = await defenceUnits.FirstOrDefaultAsync(x => x.Id.Equals(unitAttack.DefenceUnitId));
                if (attack == null || defence == null)
                    continue;
                Db.Units.Update(defence);
                var attackUnitIdWithDamage = unitAttacks.FirstOrDefault(x => x.AttackUnitId == attack.Id);
                if(attackUnitIdWithDamage == null)
                    continue;
                defence.Health -= attackUnitIdWithDamage.Damage;
                attackResult.Add(new ResultOfAttackDto {Id = defence.Id, Hp = defence.Health});
            }
            return Ok(attackResult);
        }
    }
}