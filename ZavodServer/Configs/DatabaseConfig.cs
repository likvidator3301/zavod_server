using System;
using System.IO;
using System.Text.Json;

namespace ZavodServer
{
    public class DatabaseConfig
    {
        private string path = "configs/postgresConfig.json";
        
        public string ReadConfig()
        {
            StreamReader sr;
            string textConfig;
            PostgresConfig postgresConfig = null;
            try
            {
                sr = new StreamReader(path);
                textConfig = sr.ReadToEnd();
                postgresConfig = JsonSerializer.Deserialize<PostgresConfig>(textConfig);
            }
            catch (Exception e)
            {
                textConfig = "";
                Console.WriteLine(e);
            }

            if (textConfig.Length == 0 || postgresConfig == null)
                return null;
            
            return postgresConfig.ToString();
        }
    }

    class PostgresConfig
    {
        public string host { get; set; }
        public string port { get; set; }
        public string database { get; set; }
        public string userId { get; set; }
        public string password { get; set; }
        
        public override string ToString()
        {
            return $"host={host};port={port};database={database};user id={userId};password={password};";
        }
    }
}