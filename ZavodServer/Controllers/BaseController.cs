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
        protected UserDb User { get; private set; }
        protected SessionDb Session { get; }//todo

        protected readonly DatabaseContext Db;

        public BaseController(DatabaseContext db)
        {
            this.Db = db;
        }


        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await base.OnActionExecutionAsync(context, next);

            context.HttpContext.Request.Headers.TryGetValue("token", out var token);
            var email = Cache.Cache.Get<string>(token);//хранить объект с expirationDate

            User = await Db.Users.FirstAsync(u => u.Email == email);

        }
    }

    public class SessionDb
    {
        public Guid Id;
        public Guid[] UserId;
        public int State;
    }
}