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

        /// <summary>
        ///     Возвращает конкретную сессию по Id
        /// </summary>
        /// <param name="id">Id сессии, которую хотите получить</param>
        /// <returns>Сессию с Id переданным в запросе</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SessionDb>> GetSession([FromQuery] Guid id)
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
        public ActionResult CreateNewSession([FromBody]string mapName)
        {
            var newSession = new SessionDb { Id = Guid.NewGuid(), State = SessionState.Preparing, 
                Players = new List<Player>(), GameMap = MapContainer.maps.First(x=>x.Name.Equals(mapName))};
            Db.Sessions.Add(newSession);
            return Ok();
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
            enteringSession.Players.Add(player);
            return Ok();
        }

        /// <summary>
        ///     Возвращает массив игроков из текущей сессии
        /// </summary>
        /// <returns>
        ///    Массив игроков из текущей сессии
        /// </returns>
        public ActionResult<Player> GetPlayers()
        {
            if (Session == null)
                return BadRequest();
            return Ok(Session.Players);
        }

        /// <summary>
        ///     Получает новые данные и обновляет плеера
        /// </summary>
        /// <param name="newPlayer"></param>
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
            var sessionUsers = Db.Users.Where(x => x.SessionId.Equals(sessionId));
            foreach (var user in sessionUsers)
                user.MyPlayer = null;
            Db.Sessions.Remove(deletingSession);
            return Ok();
        }
    }
}