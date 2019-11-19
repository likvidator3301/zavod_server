using Microsoft.EntityFrameworkCore;
using ZavodServer.Models;

namespace ZavodServer
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UnitDto> Units { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            DatabaseConfig dbConfig = new DatabaseConfig();
            var config = dbConfig.ReadConfig();
            if (config != null)
                optionsBuilder.UseNpgsql(config);
//            else
//                optionsBuilder.UseNpgsql(
//                    "host=localhost;port=5432;database=default;user id=default;password=default;");
        }
    }
}