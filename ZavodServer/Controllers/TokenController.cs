using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace ZavodServer.Controllers
{
    [ApiController]
    [Route("token")]
    public class TokenController : ControllerBase
    {
        /// <summary>
        ///     Updating access token
        /// </summary>
        /// <returns>
        ///    Object with new access token and scopes, and time expire
        /// </returns>
        [HttpPost]
        public ActionResult<AccessTokenDto> GetNewAccessToken([FromBody] string refreshToken)
        {
            Dictionary<string, string> body = new Dictionary<string, string>();
            GoogleAuthConfig authConfig = new GoogleAuthConfig();
            body.Add("client_id", authConfig.ReadConfig().client_id);
            body.Add("client_secret", authConfig.ReadConfig().client_secret);
            body.Add("grant_type", "refresh_token");
            body.Add("refresh_token", refreshToken);
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://oauth2.googleapis.com/");
            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var result = client.PostAsync("token", new FormUrlEncodedContent(body)).Result;
            result.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<AccessTokenDto>(result.Content.ReadAsStringAsync().Result);
        }
    }
}