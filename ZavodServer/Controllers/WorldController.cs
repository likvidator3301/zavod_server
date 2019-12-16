using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZavodServer.Models;

namespace ZavodServer.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    [Route("World")]
    public class WorldController : ControllerBase
    {
        private readonly DatabaseContext db = new DatabaseContext();

        /// <summary>
        ///     Возвразает массив юзеров с списками UnitId, BuildingId
        /// </summary>
        /// <returns>
        /// UserDb array
        /// </returns>
        [HttpGet]
        public IEnumerable<UserDb> GetWorldState() => db.Users.Select(x => x);
    }
}