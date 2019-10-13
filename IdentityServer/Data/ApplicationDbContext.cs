using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IdentityServer.Models;

namespace IdentityServer.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Location> Locations { get; set; }
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Location>().HasKey(t => t.PlaceID);
            builder.Entity<Review>().HasKey(t => new { t.PlaceID, t.UserID });
            builder.Entity<Review>().Property(t => t.OverallRating).IsRequired();
            builder.Entity<Review>().Property(t => t.DateTime).IsRequired();
            builder.Entity<Location>().HasMany(t => t.Reviews).WithOne();
            builder.Entity<User>().HasMany(t => t.Reviews).WithOne();
        }
    }
}
