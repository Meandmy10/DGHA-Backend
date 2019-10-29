using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ModelsLibrary.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        private readonly string connectionString = "Server=tcp:jtowserver.database.windows.net,1433;Initial Catalog=DevData;Persist Security Info=False;User ID=JTowers;Password=Turtle17;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Complaint> Complaints { get; set; }

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
            builder.Entity<Location>().HasIndex(b => b.PlaceID);

            builder.Entity<Review>().HasKey(t => new { t.PlaceID, t.UserID });
            builder.Entity<Review>().Property(t => t.OverallRating).IsRequired();
            builder.Entity<Review>().Property(t => t.TimeAdded).IsRequired().HasDefaultValueSql("getdate()");
            builder.Entity<Location>().HasMany(t => t.Reviews).WithOne();
            builder.Entity<User>().HasMany(t => t.Reviews).WithOne();

            builder.Entity<Complaint>().HasKey(t => new { t.PlaceID, t.UserID, t.TimeSubmitted });
            builder.Entity<Complaint>().Property(t => t.TimeSubmitted).IsRequired().HasDefaultValueSql("getdate()");
            builder.Entity<Complaint>().Property(t => t.Status).HasDefaultValue("Newly Created");
            builder.Entity<Location>().HasMany(t => t.Complaints).WithOne();
            builder.Entity<User>().HasMany(t => t.Complaints).WithOne();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
