using System;
using Microsoft.EntityFrameworkCore;
using ZavodServer.Models;

namespace ZavodServer
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UnitDb> Units { get; set; }
        public DbSet<UserDb> Users { get; set; }
        public DbSet<SessionDb> Sessions { get; set; }
        public DbSet<BagDb> Bags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbConfig = new DatabaseConfig();
            var config = dbConfig.ReadConfig();
            if (config != null)
                optionsBuilder.UseNpgsql(config);
        }
    }
}