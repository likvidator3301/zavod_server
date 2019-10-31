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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnitDto>(x =>
                x.OwnsOne(a => a.Position,
                    position =>
                    {
                        position.Property(c => c.X).HasColumnName("posX"); 
                        position.Property(c => c.Y).HasColumnName("posY"); 
                        position.Property(c => c.Z).HasColumnName("posZ"); 
                    }));
            modelBuilder.Entity<UnitDto>(x =>
                x.OwnsOne(a => a.Rotation,
                    rotation =>
                    {
                        rotation.Property(c => c.X).HasColumnName("rotX"); 
                        rotation.Property(c => c.Y).HasColumnName("rotY"); 
                        rotation.Property(c => c.Z).HasColumnName("rotZ"); 
                    }));
        }
    }
}