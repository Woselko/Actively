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

            builder.Entity<Game>()
                .HasMany(x => x.Players)
                .WithMany(x => x.Games);

            builder.Entity<Player>()
                .HasMany(x => x.Games)
                .WithMany(x => x.Players);       

            builder.Entity<SportType>()
            .HasKey(x => x.Id);

            new GameConfiguration().Configure(builder.Entity<Game>());
            new SportTypeConfiguration().Configure(builder.Entity<SportType>());
            new PlayerConfiguration().Configure(builder.Entity<Player>());

        }
    }
}
