using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZavodServer.Models;

namespace ZavodServer.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [AllowAnonymous]
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly DatabaseContext db = new DatabaseContext();
        private SignInManager<IdentityUser> signInManager;
        
        public AuthController(SignInManager<IdentityUser> signInManager)
        {
            this.signInManager = signInManager;
        }
        
        /// <summary>
        ///     Authorize person by google
        /// </summary>
        /// <returns>redirect to callback</returns>
        [HttpGet("login")]
        public ActionResult Login()
        {
            var authProp = signInManager.ConfigureExternalAuthenticationProperties("Google",
                Url.Action("LoginCallback", "Auth", null, Request.Scheme));
            return Challenge(authProp);
        }

        [HttpGet("LoginCallback")]
        public ActionResult<UserDb> LoginCallback()
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();
            var email = User.Claims.First(x => x.Type == ClaimTypes.Email).Value;
            var user = db.Users.FirstOrDefault(x => x.Email.ToLower().Equals(email.ToLower()));
            if (user != null)
                return user;
            user = new UserDb{Email = email, Id = Guid.NewGuid(), Units = new List<Guid>(), Buildings = new List<Guid>()};
            db.Users.Add(user);
            db.SaveChanges();
            return user;
        }
    }
}