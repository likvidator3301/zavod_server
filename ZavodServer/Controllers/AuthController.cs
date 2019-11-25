using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace ZavodServer.Controllers
{
    [Produces("application/json")]
    [ApiController]
//    [AllowAnonymous]
    [Route("/")]
    public class AuthController : Controller
    {
        private readonly DatabaseContext db = new DatabaseContext();
        private readonly SignInManager<ServerUserDto> signInManager;

        [HttpGet("login")]
        public ActionResult Login(string returnUrl ="/")
        {
            return new ChallengeResult(new AuthenticationProperties {RedirectUri = returnUrl});
        }

       [HttpGet("signin-google")]
        public ActionResult LoginCallback()
        {
            return Ok("Tut 4to-to napisano: "+User.Identities.First().IsAuthenticated);
        }
    }
    /*
     * http://localhost:5000/auth/googleCallback
     * ?state=CfDJ8IGASaB2OJhNiYicgRiEXj88dmjXadrbBNO_MvWxxnjJgnrW0shz1Hus4Rdxogb69fposJt1xEy5EbPnOd-gpoq3brIJdIr5HZSYu5u8kDWAmTed-8LmbDVdekjb5d4TBRQNuInVcC8PeT5kO3dua84fLwQ7kovmQ4LIQiwu7JdPCprxOYIJ4RuHJGfmTAWbttzOAY7t6152-04T9xbS4aI
     * &code=4%2FtgHEfX8-TEVWEhnZezJF83WPfSCzbJ_mfRMe437LABt9IpaphMCZd9Z8QyyXUk9h-SgafuIcsp-PEPZGBDN8qIE
     * &scope=email+profile+openid+https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fuserinfo.profile+https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fuserinfo.email
     * &authuser=0
     * &session_state=ab82c89f2feb6f6c577f42901c19d403b121bd60..3ced
     * &prompt=consent#
     */
}