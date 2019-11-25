using System.IO;
using System.Text.Json;

namespace ZavodServer
{
    public class GoogleAuthConfig
    {
        private readonly string path = "./Configs/googleConfig.json";
        public GoogleAuthConfig ReadConfig()
        {
            var configFromJson = JsonSerializer.Deserialize<GoogleAuthConfig>(File.ReadAllText(path));
            client_id = configFromJson.client_id;
            project_id = configFromJson.project_id;
            auth_uri = configFromJson.auth_uri;
            token_uri = configFromJson.token_uri;
            auth_provider_x509_cert_url = configFromJson.auth_provider_x509_cert_url;
            client_secret = configFromJson.client_secret;
            return this;
        }
        
        public string client_id { get; set; }
        public string project_id { get; set; }
        public string auth_uri { get; set; }
        public string token_uri { get; set; }
        public string auth_provider_x509_cert_url { get; set; }
        public string client_secret { get; set; }
    }
}