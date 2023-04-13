using Infrastructure.Persistence.Configuration;
using Infrastructure.Persistence.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// The entity Framework Core context class inherits from <see cref="IdentityDbContext"/>
    /// </summary>
    internal class HubDbContext : IdentityDbContext<HubIdentityUser, HubIdentityRole, string>
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
