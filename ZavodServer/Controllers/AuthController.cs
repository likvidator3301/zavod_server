using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZavodServer.Models;

namespace ZavodServer.Controllers
{
    public class AuthController : BaseController
    {
        private readonly DatabaseContext db;
        
        /// <summary>
        ///     AuthController constructor, that assign database context
        /// </summary>
        /// <param name="db">database context</param>
        public AuthController(DatabaseContext db): base(db)
        {
            this.db = db;
        }
        
        [HttpGet]
        public async Task<ActionResult<UserDb>> GetUser()
        {
            if(!HttpContext.Items.TryGetValue("email", out var emailObj))
                return BadRequest();
            var email = emailObj.ToString();
            if (!(await db.Users.AnyAsync(x => x.Email == email)))
                return Unauthorized();
            return await db.Users.FirstAsync(x => x.Email.Equals(email));
        }
    }
}