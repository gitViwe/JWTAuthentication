using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Persistance.Entity
{
    /// <summary>
    /// A custom implementation of the <see cref="IdentityUser"/>
    /// </summary>
    internal class HubIdentityUser : IdentityUser
    {
        public HubIdentityUser()
        : base()
        {
            Roles = new HashSet<IdentityUserRole<string>>();
            Claims = new HashSet<IdentityUserClaim<string>>();
            Logins = new HashSet<IdentityUserLogin<string>>();
            RefreshTokens = new HashSet<RefreshToken>();
        }

        public string Avatar { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string TOTPKey { get; set; } = string.Empty;

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
