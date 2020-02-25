using Online_FCS_Analysis.Models.Entities;
using Online_FCS_Analysis.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_FCS_Analysis.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {}
        public virtual DbSet<UserModel> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            InitiateEntity(modelBuilder);
            SeedData(modelBuilder);

        }

        private void InitiateEntity(ModelBuilder modelBuilder)
        {
            #region User
            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.id)
                    .HasColumnName("id")
                    .HasColumnType("int")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.user_email)
                    .IsRequired()
                    .HasColumnName("user_email")
                    .HasMaxLength(255);
                entity.Property(e => e.user_email)
                    .IsRequired()
                    .HasColumnName("user_email")
                    .HasMaxLength(255);
                entity.Property(e => e.user_password)
                    .IsRequired()
                    .HasColumnName("user_password")
                    .HasMaxLength(255);
                entity.Property(e => e.user_name)
                    .IsRequired()
                    .HasColumnName("user_name")
                    .HasMaxLength(255);
                entity.Property(e => e.user_role)
                    .IsRequired()
                    .HasColumnName("user_role")
                    .HasMaxLength(255);
                entity.Property(e => e.user_phone)
                    .HasColumnName("user_phone")
                    .HasMaxLength(255);
                entity.Property(e => e.user_address)
                    .HasColumnName("user_address")
                    .HasMaxLength(255);
                entity.Property(e => e.user_activated)
                    .HasColumnName("user_activated")
                    .HasDefaultValue(true)
                    .HasColumnType("bit");
            });
            #endregion User
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>().HasData(new UserModel
            {
                id = 1,
                user_name = "Admin",
                user_email = "admin@gmail.com",
                user_password = "secret",
                user_role = Constants.ROLE_ADMIN,
                user_avatar = "/uploads/avatars/admin.png"
            });
        }
    }
}
