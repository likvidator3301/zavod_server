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

        /// <summary>
        ///     Get a code and a url for auth
        /// </summary>
        /// <returns>
        ///     object with code, uri, time expire, device code
        /// </returns>
        [HttpGet]
        public ActionResult<GoogleAuthDto> GetAuthCode()
        {
            Dictionary<string, string> body = new Dictionary<string, string>();
            GoogleAuthConfig authConfig = new GoogleAuthConfig();
            body.Add("client_id", authConfig.ReadConfig().client_id);
            body.Add("scope", "email");
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://oauth2.googleapis.com/device/");
            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var result = client.PostAsync("code", new FormUrlEncodedContent(body)).Result;
            result.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<GoogleAuthDto>(result.Content.ReadAsStringAsync().Result);
        }
    }
}