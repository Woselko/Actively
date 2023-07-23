using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ActivelyDomain.Entities;

namespace ActivelyInfrastructure.EntityConfiguration
{
    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.Property(x => x.CreationDate)
                .HasColumnType("date");

            builder.HasMany(x => x.Players)
                .WithMany(x => x.Games);
        }
    }
}
