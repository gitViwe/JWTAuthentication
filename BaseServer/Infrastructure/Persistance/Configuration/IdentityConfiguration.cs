using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    /// <summary>
    /// Configuration for the ASP Net Core Identity entities
    /// </summary>
    internal class IdentityConfiguration :
        IEntityTypeConfiguration<IdentityUserLogin<Guid>>,
        IEntityTypeConfiguration<IdentityUserRole<Guid>>,
        IEntityTypeConfiguration<IdentityUserToken<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
        {
            builder.HasKey(entity => entity.UserId);
        }

        public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
        {
            builder.HasKey(entity => new { entity.UserId, entity.RoleId });
        }

        public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder)
        {
            builder.HasKey(entity => entity.UserId);
        }
    }
}
