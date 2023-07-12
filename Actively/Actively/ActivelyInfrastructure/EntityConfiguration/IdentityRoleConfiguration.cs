using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ActivelyDomain.Entities;

namespace ActivelyInfrastructure.EntityConfiguration
{
    public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
               new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
               new IdentityRole() { Name = "Developer", ConcurrencyStamp = "2", NormalizedName = "Developer" },
               new IdentityRole() { Name = "Moderator", ConcurrencyStamp = "3", NormalizedName = "Moderator" },
               new IdentityRole() { Name = "PremiumUser", ConcurrencyStamp = "4", NormalizedName = "PremiumUser" },
               new IdentityRole() { Name = "GameOwner", ConcurrencyStamp = "5", NormalizedName = "GameOwner" },
               new IdentityRole() { Name = "VerifiedUser", ConcurrencyStamp = "6", NormalizedName = "VerifiedUser" },
               new IdentityRole() { Name = "User", ConcurrencyStamp = "7", NormalizedName = "User" });
        }
    }
}
