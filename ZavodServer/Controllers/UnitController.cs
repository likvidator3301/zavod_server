using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZavodServer.Models;
using Models;

namespace ZavodServer.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    [Route("units")]
    public class UnitController : ControllerBase
    {
        private readonly DatabaseContext db = new DatabaseContext();

        /// <summary>
        ///     Returns all units id
        /// </summary>
        /// <returns>All units id</returns>
        /// <response code="200">Returns all units id</response>
        /// <response code="404">If no units in db</response>
        [HttpGet]
        public ActionResult<IEnumerable<UnitDb>> GetAll()
        {
            IEnumerable<UnitDb> result = db.Units.Select(x => x);
            return new ActionResult<IEnumerable<UnitDb>>(result);
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
            return db.Units.First(x => x.Id == id);
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
            return Vector3.Distance(db.Units.First(x => x.Id == firstUnitId).Position,
                db.Units.First(x => x.Id == secondUnitId).Position);
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
            var unitDto = db.DefaultUnits.First(x => x.Type == createUnit.UnitType).UnitDto;
            unitDto.Id = Guid.NewGuid();
            unitDto.Position = new Vector3(createUnit.Position.X, createUnit.Position.Y, createUnit.Position.Z);
            db.Units.Add(unitDto);
            var email = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            db.Users.First(x => x.Email == email)
                .Units.Add(unitDto.Id);
            db.SaveChanges();
            return unitDto;
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
            var email = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            var userDb = db.Users.First(x => x.Email == email);
            if (!userDb.Units.Contains(unitDto.Id))
                return BadRequest();
            if (!db.Units.Select(x => x.Id).Contains(unitDto.Id))
                return NotFound(unitDto);
            var updatingUnit = db.Units.First(x => x.Id == unitDto.Id);
            db.Units.Update(updatingUnit);
            updatingUnit.Copy(unitDto);
            db.SaveChanges();
            return updatingUnit;
        }

        /// <summary>
        ///     Delete unit from db
        /// </summary>
        /// <returns>Deleted unit</returns>
        /// <response code="200">Returns deleted unit</response>
        /// <response code="404">If unit not found in db</response>
        [HttpDelete]
        public ActionResult<UnitDb> DeleteUnit([FromRoute] Guid id)
        {
            var userDb = db.Users.First(x => x.Email == User.Claims.First(c => c.Type == ClaimTypes.Email).Value);
            if (!userDb.Units.Contains(id))
                return BadRequest();
            if (!db.Units.Select(x => x.Id).Contains(id))
                return NotFound(id);
            userDb.Units.Remove(id);
            db.Units.Remove(db.Units.First(x => x.Id == id));
            db.SaveChanges();
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
        public ActionResult<IEnumerable<ResultOfAttackDto>> AttackUnit([FromBody] params AttackUnitDto[] unitAttacks)
        {
            var userUnits = db.Users.First(x => x.Email == User.Claims.First(c => c.Type == ClaimTypes.Email).Value)
                .Units;
            var validatedUnits = unitAttacks.Where(x => userUnits.Contains(x.Attack)).ToList();
            List<ResultOfAttackDto> attackResult = new List<ResultOfAttackDto>();
            //Уверен что эо плохая идея, но ничего лучше пока не придумал
            foreach (var unitAttack in validatedUnits)
            {
                var attack = db.Units.First(x => x.Id == unitAttack.Attack);
                var defence = db.Units.First(x => x.Id == unitAttack.Defence);
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
            return attackResult;
        }
        
        /// <summary>
        ///     Move unit to new position
        /// </summary>
        /// <param name="moveUnits"></param>
        /// <returns></returns>
        [HttpPost("move")]
        public ActionResult<IEnumerable<MoveUnitDto>> MoveUnit([FromBody] params MoveUnitDto[] moveUnits)
        {
            var userUnits = db.Users.First(x => x.Email == User.Claims.First(c => c.Type == ClaimTypes.Email).Value)
                .Units;
            var validatedUnits = moveUnits.Where(x => userUnits.Contains(x.Id)).ToList();
            List<MoveUnitDto> badMoveResult = new List<MoveUnitDto>();
            //Уверен что эо плохая идея, но ничего лучше пока не придумал
            foreach (var movingUnit in validatedUnits)
            {
                var movesUnit = db.Units.First(x => x.Id == movingUnit.Id);
                var oldPosition = movesUnit.Position;
                var newPosition = movingUnit.NewPosition;
                if (Vector3.Distance(movesUnit.Position, newPosition) > movesUnit.MoveSpeed)
                {
                    badMoveResult.Add(new MoveUnitDto{Id = movesUnit.Id, NewPosition = oldPosition});
                    continue;
                }
                db.Units.Update(movesUnit);
                movesUnit.Position = newPosition;
                db.SaveChanges();
            }
            return badMoveResult;
        }

//        [HttpPost("CreateSome")]
//        public ActionResult<string> CreateSomeUnits([FromBody] UnitDb unitDb)
//        {
//            unitDb.Type = UnitType.Warrior;
//            db.DefaultUnits.Add(new DefaultUnitDb {Type = unitDb.Type, UnitDto = unitDb});
//            db.SaveChanges();
//            return db.DefaultUnits.First().UnitDto.ToString();
//        }
    }
}