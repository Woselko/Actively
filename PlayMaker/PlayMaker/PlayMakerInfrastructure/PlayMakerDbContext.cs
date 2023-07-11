using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlayMakerDomain.Entities;
using PlayMakerInfrastructure.EntityConfiguration;

namespace PlayMakerInfrastructure
{
    public class PlayMakerDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Game>? Game { get; set; }
        public DbSet<Player>? Player { get; set; }
        public DbSet<SportType>? SportType { get; set; }

        public PlayMakerDbContext() { }
        //public PlayMakerDbContext(DbContextOptions<PlayMakerDbContext> options) : base(options){}
        public PlayMakerDbContext(DbContextOptions options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);      

            new GameConfiguration().Configure(builder.Entity<Game>());
            new SportTypeConfiguration().Configure(builder.Entity<SportType>());
            new PlayerConfiguration().Configure(builder.Entity<Player>());
            new IdentityRoleConfiguration().Configure(builder.Entity<IdentityRole>());
        }

        //add-migration IdentityInit
        //update-database
        //string _connectionString = "Server=WOSELKO;Integrated Security = true; Initial Catalog=PlayMakerDb;Trusted_Connection=True;TrustServerCertificate=True";
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(_connectionString);
        //}
    }
}
