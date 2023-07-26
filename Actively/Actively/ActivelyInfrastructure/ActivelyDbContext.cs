using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ActivelyDomain.Entities;
using ActivelyInfrastructure.EntityConfiguration;

namespace ActivelyInfrastructure
{
    public class ActivelyDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Game>? Game { get; set; }
        public DbSet<Player>? Player { get; set; }

        public ActivelyDbContext() { }
        //public ActivelyDbContext(DbContextOptions<ActivelyDbContext> options) : base(options){}
        public ActivelyDbContext(DbContextOptions<ActivelyDbContext> options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);      

            new GameConfiguration().Configure(builder.Entity<Game>());
            new PlayerConfiguration().Configure(builder.Entity<Player>());
            new IdentityRoleConfiguration().Configure(builder.Entity<IdentityRole>());
        }

        //add-migration IdentityInit
        //update-database
        //string _connectionString = "Server=WOSELKO;Integrated Security = true; Initial Catalog=ActivelyDb;Trusted_Connection=True;TrustServerCertificate=True";
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(_connectionString);
        //}
    }
}
