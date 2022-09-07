using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    /// <summary>
    /// A custom implementation of the <see cref="IdentityUser"/>
    /// </summary>
    internal class HubIdentityUser : IdentityUser<Guid>
    {
        public HubIdentityUser()
        : base()
        {
            Roles = new HashSet<IdentityUserRole<Guid>>();
            Claims = new HashSet<IdentityUserClaim<Guid>>();
            Logins = new HashSet<IdentityUserLogin<Guid>>();
        }

        public bool IsActive { get; set; }
        public string Avatar { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public virtual ICollection<IdentityUserRole<Guid>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<Guid>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<Guid>> Logins { get; set; }
    }
}
