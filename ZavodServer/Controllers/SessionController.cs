using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using ZavodServer.Models;
using Player = ZavodServer.Models.Player;

namespace ZavodServer.Controllers
{
    public class SessionController : BaseController
    {
        public SessionController(DatabaseContext db) : base(db) {}

        /// <summary>
        ///     Возвращает все существующие на сервере игровые сессии
        /// </summary>
        /// <returns>
        ///    Список всех существующих на сервере игровых сессий
        /// </returns>
        [HttpGet]
        public ActionResult<IEnumerable<SessionDb>> GetAllSessions() => Ok(Db.Sessions.Select(x => x));

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<SessionDb>>> GetSession([FromQuery] Guid id)
        {
            var result = await Db.Sessions.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (result == null)
                return NotFound();
            return Ok(result);
        } 

        /// <summary>
        ///     Создаёт новую сессию
        /// </summary>
        /// <returns>
        ///    Новую сессию
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<SessionDb>> CreateNewSession()
        {
            var player = new Player {LastTimeActivity = DateTimeOffset.UtcNow, User = UserDb};
            var newSession = new SessionDb { Id = Guid.NewGuid(), State = SessionState.Preparing, Players = new List<Player>{player}};
            UserDb.SessionId = newSession.Id;
            Db.Sessions.Add(newSession);
            return Ok(newSession);
        }

        /// <summary>
        ///     Добавляет в сессию игрока
        /// </summary>
        /// <param name="sessionId">
        ///    Id сессии
        /// </param>
        /// <returns>
        ///    Сессию с новым игроком
        /// </returns>
        [HttpPost("enter")]
        public async Task<ActionResult<SessionDb>> EnterSession([FromQuery] Guid sessionId)
        {
            var player = new Player {LastTimeActivity = DateTimeOffset.UtcNow, User = UserDb};
            var enteringSession = await Db.Sessions.FirstOrDefaultAsync(x => 
                x.Id.Equals(sessionId) && x.State.Equals(SessionState.Preparing));
            if (enteringSession == null)
                return BadRequest();
            Db.Sessions.Update(enteringSession);
            UserDb.SessionId = enteringSession.Id;
            enteringSession.Players.Add(player);
            return Ok(enteringSession);
        }

        /// <summary>
        ///     Запускает игровую сессию (меняет в ней 1 аттрибут)
        /// </summary>
        /// <param name="sessionId">
        ///    Id сессии
        /// </param>
        /// <returns>
        ///    Сессию с игроками и изменённым статусом
        /// </returns>
        [HttpPost("start")]
        public async Task<ActionResult<SessionDb>> StartSession([FromQuery] Guid sessionId)
        {
            var enteringSession = await Db.Sessions.FirstOrDefaultAsync(x => 
                x.Id.Equals(sessionId) && x.State.Equals(SessionState.Preparing));
            if (enteringSession == null)
                return BadRequest();
            Db.Sessions.Update(enteringSession);
            enteringSession.State = SessionState.InGame;
            return Ok(enteringSession);
        }

        /// <summary>
        ///     Удаляет игровую сессию
        /// </summary>
        /// <param name="sessionId">
        ///    Id сессии
        /// </param>
        /// <returns>
        ///    Возвращает статус удалилось ли
        /// </returns>
        [HttpDelete]
        public async Task<ActionResult> DeleteSession([FromBody] Guid sessionId)
        {
            var deletingSession = await Db.Sessions.FirstOrDefaultAsync(x => x.Id.Equals(sessionId)); 
            if (deletingSession == null)
                return BadRequest();
            foreach (var player in deletingSession.Players)
            {
                Db.Users.Update(player.User);
                player.User.SessionId = Guid.Empty;
            }
            Db.Sessions.Remove(deletingSession);
            return Ok();
        }
    }
}