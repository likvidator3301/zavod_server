using System;
using Microsoft.EntityFrameworkCore;
using ZavodServer.Models;

namespace ZavodServer
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UnitDb> Units { get; }
        public DbSet<UserDb> Users { get; }
        public DbSet<SessionDb> Sessions { get; }
        public DbSet<BagDb> Bags { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbConfig = new DatabaseConfig();
            var config = dbConfig.ReadConfig();
            if (config != null)
                optionsBuilder.UseNpgsql(config);
        }
    }
}