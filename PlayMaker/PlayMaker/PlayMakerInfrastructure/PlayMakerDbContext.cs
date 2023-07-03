using Microsoft.EntityFrameworkCore;
using PlayMakerDomain.Entities;
using PlayMakerInfrastructure.EntityConfiguration;

namespace PlayMakerInfrastructure
{
    public class PlayMakerDbContext : DbContext
    {
        public DbSet<Game> Game { get; set; }
        public DbSet<Player> Player { get; set; }
        public DbSet<SportType> SportType { get; set; }

        public PlayMakerDbContext(DbContextOptions<PlayMakerDbContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);      

            new GameConfiguration().Configure(builder.Entity<Game>());
            new SportTypeConfiguration().Configure(builder.Entity<SportType>());
            new PlayerConfiguration().Configure(builder.Entity<Player>());

        }

        //
        //update-database
        //string _connectionString = "Server=WOSELKO;Integrated Security = true; Initial Catalog=PlayMakerDb;Trusted_Connection=True;TrustServerCertificate=True";
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(_connectionString);
        //}
    }
}
