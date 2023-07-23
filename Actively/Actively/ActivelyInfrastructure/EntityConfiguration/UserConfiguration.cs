using ActivelyDomain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivelyInfrastructure.EntityConfiguration
{
    public class UserConfiguration
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(maxLength: 50);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Gender)
                .IsRequired();

            builder.HasKey(x => x.Id);
        }
    }
}
