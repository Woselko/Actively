using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlayMakerDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PlayMakerInfrastructure.EntityConfiguration
{
    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.Property(x => x.GameDate)
                .HasColumnType("date");

            builder.Property(x => x.CreationDate)
                .HasColumnType("date");

            builder.HasMany(x => x.Players)
                .WithMany(x => x.Games);
        }
    }
}
