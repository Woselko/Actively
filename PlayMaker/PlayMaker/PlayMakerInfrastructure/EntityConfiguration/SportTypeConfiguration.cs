using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlayMakerDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayMakerInfrastructure.EntityConfiguration
{
    public class SportTypeConfiguration : IEntityTypeConfiguration<SportType>
    {
        public void Configure(EntityTypeBuilder<SportType> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(x => x.Id)
                 .HasColumnName("ID");
        }
    }
}
