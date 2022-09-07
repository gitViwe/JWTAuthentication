using Infrastructure.Identity;
using Infrastructure.Persistance.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance
{
    /// <summary>
    /// The entity Framework Core context class inherits from <see cref="IdentityDbContext"/>
    /// </summary>
    internal class HubDbContext : IdentityDbContext<HubIdentityUser, HubIdentityRole, Guid>
    {
        /// <summary>
        /// Instantiate context using user specified provider
        /// </summary>
        /// <param name="options">The options used by <see cref="DbContext"/></param>
        public HubDbContext(DbContextOptions<HubDbContext> options)
            : base(options)
        {
            RefreshTokens = Set<RefreshToken>();
        }

        /// <summary>
        /// Configures the database context class models specified in <see cref="IdentityConfiguration"/>
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // configure identity relations
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
