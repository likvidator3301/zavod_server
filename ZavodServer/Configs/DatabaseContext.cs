using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ZavodServer.Models;

namespace ZavodServer
{
    public class DatabaseContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<UnitDb> Units { get; set; }
        public DbSet<DefaultUnitDb> DefaultUnits { get; set; }
        public DbSet<BuildingDb> Buildings { get; set; }
        public DbSet<DefaultBuildingDb> DefaultBuildings { get; set; }
        public DbSet<UserDb> Users { get; set; }

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