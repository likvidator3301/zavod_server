using System.IO;
using System.Text.Json;

namespace ZavodServer
{
    public class DatabaseConfig
    {
        private readonly string path = "./Configs/postgresConfig.json";

        public string ReadConfig()
        {
            var postgresConfig = JsonSerializer.Deserialize<PostgresConfig>(File.ReadAllText(path));
            return postgresConfig.ToString();
        }
    }

    internal class PostgresConfig
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            return $"host={Host};port={Port};database={Database};user id={UserId};password={Password};";
        }
    }
}