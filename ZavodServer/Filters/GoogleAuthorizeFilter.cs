using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

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
             context.Result = new BadRequestResult();
        }

        private static bool IsTokenFresh(ActionContext context)
        {
            HttpClient client = new HttpClient();
            var uri = new UriBuilder("https://www.googleapis.com/oauth2/v2/userinfo");
            context.HttpContext.Request.Headers.TryGetValue("token", out var token);
            uri.Query = "access_token="+token;
            var result = client.GetAsync(uri.Uri).Result;
            var email = JsonSerializer
                .Deserialize<GoogleEmailScope>(result.Content.ReadAsStringAsync().Result)
                .email;
            context.HttpContext.Items.Add("email", email);
            return result.IsSuccessStatusCode;
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