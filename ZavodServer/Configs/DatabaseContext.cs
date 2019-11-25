using Microsoft.EntityFrameworkCore;
using Models;

namespace ZavodServer
{
    public class DatabaseContext : DbContext
    {
        public DbSet<ServerUnitDto> Units { get; set; }
        public DbSet<DefaultServerUnitDto> DefaultUnits { get; set; }
        public DbSet<ServerBuildingDto> Buildings { get; set; }
        public DbSet<DefaultSeverBuildDto> DefaultBuildings { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbConfig = new DatabaseConfig();
            var config = dbConfig.ReadConfig();
            if (config != null)
                optionsBuilder.UseNpgsql(config);
//            else
//                optionsBuilder.UseNpgsql(
//                    "host=localhost;port=5432;database=default;user id=default;password=default;");
        }
    }
}