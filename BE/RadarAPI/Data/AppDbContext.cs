using Microsoft.EntityFrameworkCore;
using RadarAPI.Models.Entities;

namespace RadarAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityParticipant> ActivityParticipants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite Key cho ActivityParticipant
            modelBuilder.Entity<ActivityParticipant>()
                .HasKey(ap => new { ap.ActivityId, ap.UserId });

            // Quan hệ 1-N: Category -> Activities
            modelBuilder.Entity<Activity>()
                .HasOne(a => a.Category)
                .WithMany()
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ 1-N: Activity -> ActivityParticipants
            modelBuilder.Entity<ActivityParticipant>()
                .HasOne(ap => ap.Activity)
                .WithMany(a => a.Participants)
                .HasForeignKey(ap => ap.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
