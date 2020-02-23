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
    public class WorldController : BaseController
    {
        private readonly DatabaseContext db;

        /// <summary>
        ///     WorldController constructor, that assign database context
        /// </summary>
        /// <param name="db">database context</param>
        public WorldController(DatabaseContext db)
        {
            this.db = db;
        }

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