using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ZavodServer.Filters;
using ZavodServer.Models;

namespace ZavodServer.Controllers
{
    [Produces("application/json")]
    [ApiController]    
    [GoogleAuthorizeFilter]
    public class BaseController: Controller
    {
        /// <summary>
        ///     DatabaseContext for others child controllers
        /// </summary>
        protected readonly DatabaseContext Db;

        protected UserDb userDb { get; private set; }
        public BaseController(DatabaseContext db)
        {
            Db = db;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            HttpContext.Items.TryGetValue("email", out var emailObj);
            var email = emailObj?.ToString();
            userDb = await Db.Users.FirstAsync(x => x.Email == email);
            await next();
        }
    }
}