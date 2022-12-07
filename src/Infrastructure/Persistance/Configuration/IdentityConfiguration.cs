using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    /// <summary>
    /// Configuration for the ASP Net Core Identity entities
    /// </summary>
    internal class IdentityConfiguration :
        IEntityTypeConfiguration<IdentityUserLogin<string>>,
        IEntityTypeConfiguration<IdentityUserRole<string>>,
        IEntityTypeConfiguration<IdentityUserToken<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
        {
            builder.HasKey(entity => entity.UserId);
        }

        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasKey(entity => new { entity.UserId, entity.RoleId });
        }

        public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
        {
            builder.HasKey(entity => entity.UserId);
        }
    }
}
