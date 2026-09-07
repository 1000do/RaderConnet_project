using Microsoft.EntityFrameworkCore;
using UserAPI.Models.Entities;

namespace UserAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔥 USER
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.UserId).HasColumnName("user_id");

                // --- THÊM RÀNG BUỘC UNIQUE CHO EMAIL Ở ĐÂY ---
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Email).HasColumnName("email").IsRequired();

                entity.Property(u => u.PasswordHash).HasColumnName("password_hash");
                entity.Property(u => u.Status).HasColumnName("status");
                entity.Property(u => u.CreatedAt).HasColumnName("created_at");
            });

            // 🔥 ROLE
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");
                entity.HasKey(r => r.RoleId);
                entity.Property(r => r.RoleId).HasColumnName("role_id");
                entity.Property(r => r.RoleName).HasColumnName("role_name");
                entity.HasMany(r => r.UserRoles).WithOne(ur => ur.Role).HasForeignKey(ur => ur.RoleId);
            });

            // 🔥 PROFILE (1-1)
            modelBuilder.Entity<Profile>(entity =>
            {
                entity.ToTable("profiles");
                entity.HasKey(p => p.UserId);
                entity.Property(p => p.UserId).HasColumnName("user_id");

                // Ràng buộc Username cũng nên là duy nhất
                entity.HasIndex(p => p.Username).IsUnique();

                entity.Property(p => p.Username).HasColumnName("username");
                entity.Property(p => p.FullName).HasColumnName("full_name");
                entity.Property(p => p.PhoneNumber).HasColumnName("phone_number");
                entity.Property(p => p.AvatarUrl).HasColumnName("avatar_url");
                entity.Property(p => p.Bio).HasColumnName("bio");
                entity.Property(p => p.Gender).HasColumnName("gender");

                entity.Property(p => p.IsPublicEmail).HasColumnName("is_public_email");
                entity.Property(p => p.IsPublicPhone).HasColumnName("is_public_phone");
                entity.Property(p => p.IsPublicProfile).HasColumnName("is_public_profile");
            });

            // 🔥 USER_ROLE (many-to-many)
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("user_roles");
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });
                entity.Property(ur => ur.UserId).HasColumnName("user_id");
                entity.Property(ur => ur.RoleId).HasColumnName("role_id");
            });
        }
    }
}