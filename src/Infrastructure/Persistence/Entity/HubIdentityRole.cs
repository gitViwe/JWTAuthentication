using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Persistence.Entity;

/// <summary>
/// A custom implementation of the <see cref="IdentityRole"/>
/// </summary>
internal class HubIdentityRole : IdentityRole
{
    internal HubIdentityRole()
    : base()
    {
        RoleClaims = new HashSet<IdentityRoleClaim<string>>();
    }

    internal HubIdentityRole(string roleName)
        : base(roleName)
    {
        Description = string.Empty;
        RoleClaims = new HashSet<IdentityRoleClaim<string>>();
    }

    internal HubIdentityRole(string roleName, string roleDescription)
    : base(roleName)
    {
        Description = roleDescription;
        RoleClaims = new HashSet<IdentityRoleClaim<string>>();
    }

    public string Description { get; set; } = string.Empty;
    public virtual ICollection<IdentityRoleClaim<string>> RoleClaims { get; set; }
}
