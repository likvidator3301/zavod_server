using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace ZavodServer.Filters
{
    public class GoogleAuthorizeFilter: Attribute, IAuthorizationFilter
    {
        /// <summary>
        ///     Authorization method, that set context.Result, when no authorize
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if(!IsTokenFresh(context))
             context.Result = new UnauthorizedResult();
        }

        private static bool IsTokenFresh(ActionContext context)
        {
            context.HttpContext.Request.Headers.TryGetValue("token", out var token);

            if (!Cache.LocalCache.TryGetValue(token, out _))
            {
                var client = new HttpClient();
                var uri = new UriBuilder("https://www.googleapis.com/oauth2/v2/userinfo")
                {
                    Query = "access_token=" + token
                };
                var result = client.GetAsync(uri.Uri).Result;
                if (!result.IsSuccessStatusCode)
                    return false;
                
                var email = JsonSerializer
                    .Deserialize<GoogleEmailScope>(result.Content.ReadAsStringAsync().Result)
                    .email;
                Cache.LocalCache.Set(token, email, TimeSpan.FromMinutes(10));// expiration*2
            }

            return true;
        }
    }
    
    class GoogleEmailScope
    {
        public string id { get; set; }
        public string email { get; set; }
        public bool verified_email { get; set; }
        public string picture { get; set; }
    }
}