using System;
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
    [Route("/")]
//    [Authorize]
    public class AuthController : Controller
    {
        private readonly DatabaseContext db = new DatabaseContext();
        private SignInManager<IdentityUser> signInManager;
        private UserManager<IdentityUser> userManager;
        
        public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        
        [HttpGet("login")]
        public ActionResult Login(string returnUrl ="/LoginCallback")
        {
            var authProp = signInManager.ConfigureExternalAuthenticationProperties("Google",
                Url.Action("LoginCallback", "Auth", null, Request.Scheme));
            return Challenge(authProp);
        }

        [HttpGet("LoginCallback")]
        public async Task<ActionResult> LoginCallback()
        {
//            var info = await signInManager.GetExternalLoginInfoAsync();
//            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            var email = User.Claims.First(x => x.Type == ClaimTypes.Email).Value;
            var name = User.Identity.Name.Trim();
            var user = new IdentityUser {Email = email, UserName = name, Id = Guid.NewGuid().ToString()};
            var result = await userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                // установка куки
                await signInManager.SignInAsync(user, false);
                return RedirectToAction("GetAll", "Unit");
            }
            return Ok(email);
        }
    }
}