using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        ///     Get a code and a url for auth
        /// </summary>
        /// <returns>
        ///     object with code, uri, time expire, device code
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<GoogleAuthDto>> GetAuthCode()
        {
            var body = new Dictionary<string, string>();
            var googleAuthConfig = new GoogleAuthConfig();
            body.Add("client_id", googleAuthConfig.ReadConfig().client_id);
            body.Add("scope", "email");
            var client = new HttpClient {BaseAddress = new Uri("https://oauth2.googleapis.com/device/")};
            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var result = await client.PostAsync("code", new FormUrlEncodedContent(body));
            result.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<GoogleAuthDto>(await result .Content.ReadAsStringAsync());
        }

        /// <summary>
        ///     Updating access token
        /// </summary>
        /// <returns>
        ///    Object with new access token and scopes, and time expire
        /// </returns>
        [HttpPost("refreshToken")]
        public async Task<ActionResult<AccessTokenDto>> GetNewAccessToken([FromBody] string refreshToken)
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
            var result = await client.PostAsync("token", new FormUrlEncodedContent(body));
            if(result.IsSuccessStatusCode)
                return Ok(JsonSerializer.Deserialize<AccessTokenDto>(await result.Content.ReadAsStringAsync()));
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
        public async Task<ActionResult<PollingResult>> PollGoogle([FromBody] string deviceCode)
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
            var result = await client.PostAsync("token", new FormUrlEncodedContent(body));
            var statusCode = Status.Ok;
            switch (result.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    statusCode = Status.InvalidGrant;
                    break;
                case HttpStatusCode.Unauthorized:
                    statusCode = Status.InvalidClient;
                    break;
                case HttpStatusCode.PreconditionRequired:
                    statusCode = Status.AuthorizationPending;
                    break;
                case HttpStatusCode.Forbidden:
                    var error =
                        JsonSerializer.Deserialize<GoogleAuthError>(await result.Content.ReadAsStringAsync()).error;
                    statusCode = error == "access_denied" ? Status.AccessDenied : Status.PollingTooFrequently;                    
                    break;
            }
            if (!result.IsSuccessStatusCode) return Unauthorized();
            var tokens =
                JsonSerializer.Deserialize<AccessAndRefreshTokenDto>(await result.Content.ReadAsStringAsync());
            var user = await Register(await GetEmailFromGoogle(tokens.access_token));
            if (user == null)
                return Unauthorized();
            return Ok(new PollingResult { Tokens = tokens, User = user, Status = statusCode});
        }
        
        private async Task<UserDb> Register(string email)
        {
            if (email == null)
                return null;
            var user = await db.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));
            if (user != null)
                return user;
            user = new UserDb{Email = email, Id = Guid.NewGuid(), Currencies = new List<Currency>(), SessionId = Guid.Empty};
            db.Users.Add(user);
            return user;
        }

        private static async Task<string> GetEmailFromGoogle(string accessToken)
        {
            var client = new HttpClient();
            var uri = new UriBuilder("https://www.googleapis.com/oauth2/v2/userinfo");
            uri.Query = "access_token="+accessToken;
            var result = await client.GetAsync(uri.Uri);
            if(result.IsSuccessStatusCode)
                return JsonSerializer
                    .Deserialize<GoogleEmailScope>(await result.Content.ReadAsStringAsync())
                    .email;
            return null;
        }
    }
}