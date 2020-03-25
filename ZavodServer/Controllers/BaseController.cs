using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ZavodServer.Filters;
using ZavodServer.Models;

namespace ZavodServer.Controllers
{
    [GoogleAuthorizeFilter]
    [Produces("application/json")]
    [ApiController]
    [Route("{controller}")]

    public class BaseController : Controller
    {
        protected UserDb UserDb { get; private set; }
        protected SessionDb Session { get; private set; }

        protected readonly DatabaseContext Db;

        public BaseController(DatabaseContext db)
        {
            Db = db;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.Request.Headers.TryGetValue("token", out var token);
            var email = Cache.LocalCache.Get<string>(token); //хранить объект с expirationDate

            UserDb = await Db.Users.FirstAsync(u => u.Email == email);
            Session = await Db.Sessions.FirstOrDefaultAsync(x => x.Id.Equals(UserDb.SessionId));
            await next();
            if (Session != null)
                Session.Players.First(x => x.User.Id.Equals(UserDb.Id)).LastTimeActivity = DateTimeOffset.Now;
            await Db.SaveChangesAsync();
        }
    }
}