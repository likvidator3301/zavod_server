using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZavodServer.Models;

namespace ZavodServer.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("auth")]
    public class AuthController : BaseController
    {
        private readonly DatabaseContext db;
        
        /// <summary>
        ///     AuthController constructor, that assign database context
        /// </summary>
        /// <param name="db">database context</param>
        public AuthController(DatabaseContext db)
        {
            this.db = db;
        }

        /// <summary>
        ///     Method that register new user or returns existing
        /// </summary>
        /// <returns></returns>
        [HttpGet("Register")]
        public ActionResult<UserDb> Register()
        {
            if(!HttpContext.Items.TryGetValue("email", out var emailObj))
                return BadRequest();
            var email = emailObj.ToString();
            var user = db.Users.FirstOrDefault(x => x.Email.ToLower().Equals(email.ToLower()));
            if (user != null)
                return user;
            user = new UserDb{Email = email, Id = Guid.NewGuid(), Units = new List<Guid>(), Buildings = new List<Guid>()};
            db.Users.Add(user);
            db.SaveChanges();
            return Ok(user);
        }
    }
}