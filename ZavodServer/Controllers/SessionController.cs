using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public ActionResult<IEnumerable<SessionDb>> GetAllSessions()
        {
            return Ok(Db.Sessions.ToList());
        } 

        /// <summary>
        ///     Возвращает конкретную сессию по Id
        /// </summary>
        /// <param name="id">Id сессии, которую хотите получить</param>
        /// <returns>Сессию с Id переданным в запросе</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SessionDb>> GetSession([FromRoute] Guid id)
        {
            var result = await Db.Sessions.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (result == null)
                return NotFound();
            return Ok(result);
        } 

        /// <summary>
        ///     Создаёт новую сессию
        /// </summary>
        [HttpPost]
        public ActionResult<Guid> CreateNewSession([FromBody]string mapName)
        {
            var newSession = new SessionDb { Id = Guid.NewGuid(), State = SessionState.Preparing, 
                Players = new List<Player>(), GameMap = MapContainer.maps.First(x=>x.Name.Equals(mapName))};
            Db.Sessions.Add(newSession);
            return Ok(newSession.Id);
        }

        /// <summary>
        ///     Добавляет в сессию игрока
        /// </summary>
        /// <param name="enterSessionRequest">
        ///    Объект с именем игрока и id сессии
        /// </param>
        [HttpPost("enter")]
        public async Task<ActionResult> EnterSession([FromBody] EnterSessionRequest enterSessionRequest)
        {
            var player = new Player {LastTimeActivity = DateTimeOffset.UtcNow, 
                Nickname = enterSessionRequest.Nickname, Id = Guid.NewGuid()};
            var enteringSession = await Db.Sessions.FirstOrDefaultAsync(x => 
                x.Id.Equals(enterSessionRequest.SessionId) && x.State.Equals(SessionState.Preparing));
            if (enteringSession == null)
                return NotFound();
            Db.Sessions.Update(enteringSession);
            UserDb.SessionId = enteringSession.Id;
            UserDb.MyPlayer = player;
            enteringSession.Players.Add(player);
            return Ok();
        }

        /// <summary>
        ///     Удаляет из сессии игрока
        /// </summary>
        /// <param name="sessionId">
        ///    Id сессии
        /// </param>
        [HttpPost("leave")]
        public async Task<ActionResult> LeaveSession([FromBody] Guid sessionId)
        {
            var leavingSession = await Db.Sessions.FirstOrDefaultAsync(x => x.Id.Equals(sessionId));
            if (leavingSession == null)
                return NotFound();
            Db.Sessions.Update(leavingSession);
            leavingSession.Players.Remove(UserDb.MyPlayer);
            UserDb.SessionId = Guid.Empty;
            UserDb.MyPlayer = null;
            return Ok();
        }

        /// <summary>
        ///     Возвращает массив игроков из текущей сессии
        /// </summary>
        /// <returns>
        ///    Массив игроков из текущей сессии
        /// </returns>
        [HttpGet("players")]
        public ActionResult<IEnumerable<Player>> GetPlayers()
        {
            if (Session == null)
                return BadRequest();
            return Ok(Session.Players);
        }

        /// <summary>
        ///     Получает новые данные и обновляет плеера
        /// </summary>
        /// <param name="newPlayer"></param>
        [HttpPost("player")]
        public ActionResult UpdatePlayerState([FromBody] InputPlayerModel newPlayer)
        {
            UserDb.MyPlayer.Requisites = newPlayer.Requisites;
            UserDb.MyPlayer.Resources = newPlayer.Resources;
            return Ok();
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
        public async Task<ActionResult<HttpStatusCode>> StartSession([FromBody] Guid sessionId)
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
        [HttpDelete("{sessionId}")]
        public async Task<ActionResult> DeleteSession([FromRoute] Guid sessionId)
        {
            var deletingSession = await Db.Sessions.FirstOrDefaultAsync(x => x.Id.Equals(sessionId)); 
            if (deletingSession == null)
                return BadRequest();
            var sessionUsers = Db.Users.Where(x => x.SessionId.Equals(sessionId));
            var usersId = await sessionUsers.Select(x => x).ToListAsync();
            foreach (var userId in usersId)
            {
                var units = await Db.Units.Where(x => x.PlayerId.Equals(userId.MyPlayer.Id)).ToListAsync();
                Db.Units.RemoveRange(units);
            }
            foreach (var user in sessionUsers)
            {
                user.MyPlayer = null;
                user.SessionId = Guid.Empty;
            }
            Db.Sessions.Remove(deletingSession);
            await Db.SaveChangesAsync();
            return Ok();
        }
    }
}