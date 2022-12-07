using Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Identity
{
    /// <summary>
    /// Holds the information used to request a new JWT token
    /// </summary>
    internal class RefreshToken : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string JwtId { get; set; } = string.Empty;
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        [ForeignKey(nameof(UserId))]
        public HubIdentityUser User { get; set; } = new();
    }
}
