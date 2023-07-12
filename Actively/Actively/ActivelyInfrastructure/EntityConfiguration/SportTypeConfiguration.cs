using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ActivelyDomain.Entities;

namespace ActivelyInfrastructure.EntityConfiguration
{
    public class SportTypeConfiguration : IEntityTypeConfiguration<SportType>
    {
        public void Configure(EntityTypeBuilder<SportType> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(25);

            builder.HasKey(x => x.Id);
        }
    }
}
