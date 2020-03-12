using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using ZavodServer.Filters;
using ZavodServer.Models;

namespace ZavodServer.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
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

        [HttpGet]
        [GoogleAuthorizeFilter]
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