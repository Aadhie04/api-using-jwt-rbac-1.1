using APIUsingJWT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Data.DbEntities
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext ()
        {

        }
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Email)
                .IsUnicode(false);
                entity.Property(e => e.PasswordHash)
                .HasColumnType("nvarchar(max)");
                entity.HasIndex(e => e.UserName)
                .IsUnique();
                entity.HasIndex(e => e.Email)
                .IsUnique();
            });
            modelBuilder.Entity<Staff>(entity =>
            {
                entity.Property(e => e.FullName)
                .IsUnicode(false);
                entity.Property(e => e.Email) 
                .IsUnicode(false);
                //entity.Property(e => e.PasswordHash)
                //.HasColumnType("nvarchar(max)");
                entity.Property(e => e.Address)
               .IsUnicode(false);
                entity.HasIndex(e => e.Email)
                .IsUnique();
            });
            modelBuilder.Entity<Article>(entity =>
            {
                entity.Property(e => e.Title)
                .IsUnicode(false);
                entity.Property(e => e.Content)
                .IsUnicode(false);
                entity.Property(e => e.ImagePath)
                .IsUnicode(false);

                 entity.HasOne(a => a.User)
                .WithMany(u => u.Articles)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Advertisement>(entity => 
            {
                entity.Property(e => e.Title)
                .IsUnicode(false);
                entity.Property(e => e.Description)
               .IsUnicode(false);
                entity.Property(e => e.ImagePath)
               .IsUnicode(false);

                entity.HasOne(ad => ad.Staff)
                .WithMany(s => s.Advertisements)
                .HasForeignKey(ad => ad.StaffId)
                .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });
            });
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });
            });
        }
    }

}
