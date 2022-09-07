using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    /// <summary>
    /// A custom implementation of the <see cref="IdentityRole"/>
    /// </summary>
    internal class HubIdentityRole : IdentityRole<Guid>
    {
        internal HubIdentityRole()
        : base()
        {
            RoleClaims = new HashSet<IdentityRoleClaim<Guid>>();
        }

        internal HubIdentityRole(string roleName)
            : base(roleName)
        {
            Description = string.Empty;
            RoleClaims = new HashSet<IdentityRoleClaim<Guid>>();
        }

        internal HubIdentityRole(string roleName, string roleDescription)
        : base(roleName)
        {
            Description = roleDescription;
            RoleClaims = new HashSet<IdentityRoleClaim<Guid>>();
        }

        public string Description { get; set; } = string.Empty;
        public virtual ICollection<IdentityRoleClaim<Guid>> RoleClaims { get; set; }
    }
}
