using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Models;
using ZavodServer.Filters;
using ZavodServer.Models;
using PollingResult = ZavodServer.Models.PollingResult;

namespace ZavodServer.Controllers
{
    [ApiController]
    [Route("token")]
    public class TokenController : ControllerBase
    {
        private readonly DatabaseContext db;
        private readonly GoogleAuthConfig authConfig;
        /// <summary>
        ///     TokenController constructor, that assign database context and google config
        /// </summary>
        /// <param name="db">database context</param>
        public TokenController(DatabaseContext db)
        {
            this.db = db;
            authConfig = new GoogleAuthConfig();
        }
        
        /// <summary>
        ///     Updating access token
        /// </summary>
        /// <returns>
        ///    Object with new access token and scopes, and time expire
        /// </returns>
        [HttpPost("refreshToken")]
        public ActionResult<AccessTokenDto> GetNewAccessToken([FromBody] string refreshToken)
        {
            var body = new Dictionary<string, string>
            {
                {"client_id", authConfig.ReadConfig().client_id},
                {"client_secret", authConfig.ReadConfig().client_secret},
                {"grant_type", "refresh_token"},
                {"refresh_token", refreshToken}
            };
            var client = new HttpClient {BaseAddress = new Uri("https://oauth2.googleapis.com/")};
            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var result = client.PostAsync("token", new FormUrlEncodedContent(body)).Result;
            if(result.IsSuccessStatusCode)
                return Ok(JsonSerializer.Deserialize<AccessTokenDto>(result.Content.ReadAsStringAsync().Result));
            return Unauthorized();
        }
        
        /// <summary>
        ///     Polling Google to take tokens
        /// </summary>
        /// <param name="deviceCode">
        ///    Code from GetAuth in AuthController
        /// </param>
        /// <returns>
        ///    Object with new user and tokens
        /// </returns>
        [HttpPost("pollGoogle")]
        public ActionResult<PollingResult> PollGoogle([FromBody] string deviceCode)
        {
            var body = new Dictionary<string, string>
            {
                {"client_id", authConfig.ReadConfig().client_id},
                {"client_secret", authConfig.ReadConfig().client_secret},
                {"device_code", deviceCode},
                {"grant_type", "urn:ietf:params:oauth:grant-type:device_code"}
            };
            var client = new HttpClient {BaseAddress = new Uri("https://oauth2.googleapis.com/")};
            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var result = client.PostAsync("token", new FormUrlEncodedContent(body)).Result;
            if (result.IsSuccessStatusCode)
            {
                var tokens =
                    JsonSerializer.Deserialize<AccessAndRefreshTokeDto>(result.Content.ReadAsStringAsync().Result);
                var user = Register(GetEmailFromGoogle(tokens.access_token));
                if (user == null)
                    return Unauthorized();
                return Ok(new PollingResult { Tokens = tokens, User = user});
            }
            return Unauthorized();
        }
        
        private UserDb Register(string email)
        {
            if (email == null)
                return null;
            var user = db.Users.FirstOrDefault(x => x.Email.ToLower().Equals(email.ToLower()));
            if (user != null)
                return user;
            user = new UserDb{Email = email, Id = Guid.NewGuid(), Units = new List<Guid>(), Buildings = new List<Guid>()};
            db.Users.Add(user);
            db.SaveChanges();
            return user;
        }

        private string GetEmailFromGoogle(string accessToken)
        {
            HttpClient client = new HttpClient();
            var uri = new UriBuilder("https://www.googleapis.com/oauth2/v2/userinfo");
            uri.Query = "access_token="+accessToken;
            var result = client.GetAsync(uri.Uri).Result;
            if(result.IsSuccessStatusCode)
                return JsonSerializer
                    .Deserialize<GoogleEmailScope>(result.Content.ReadAsStringAsync().Result)
                    .email;
            return null;
        }
    }
}