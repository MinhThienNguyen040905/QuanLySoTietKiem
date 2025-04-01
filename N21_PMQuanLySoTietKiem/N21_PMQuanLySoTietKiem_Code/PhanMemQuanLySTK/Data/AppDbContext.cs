using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhanMemQuanLySTK.Models;

namespace PhanMemQuanLySTK.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Users> Users{ get; set; }
        public DbSet<Interest_Rates> Interest_Rates { get; set; }
        public DbSet<Personal_Savings_Accounts> Personal_Savings_Accounts { get; set; }
        public DbSet<Personal_Notifications> Personal_Notifications { get; set; }
        public DbSet<Personal_Transactions_Information> Personal_Transactions_Information { get; set; }
        public DbSet<Group_Details> Group_Details { get; set; }
        public DbSet<Group_Notifications> Group_Notifications { get; set; }
        public DbSet<Group_Notifications_Details> Group_Notifications_Details { get; set; }
        public DbSet<Group_Savings_Accounts> Group_Savings_Accounts { get; set; }
        public DbSet<Group_Transactions_Information> Group_Transactions_Information { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(u => u.Username)
                      .HasMaxLength(256) 
                      .IsRequired();

                entity.HasKey(u => u.Username);
            });

            modelBuilder.Entity<Interest_Rates>(entity =>
            {
                entity.HasKey(p => p.Interest_Rate_ID);

                entity.Property(p => p.Interest_Rate_ID)
                      .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Personal_Savings_Accounts>(entity =>
            {
                entity.HasKey(p => p.Saving_ID);

                entity.Property(p => p.Saving_ID)
                      .ValueGeneratedOnAdd();

                entity.HasOne(p => p.User)
                      .WithMany(u => u.Personal_Savings_Accounts)
                      .HasForeignKey(p => p.Username)
                      .HasPrincipalKey(u => u.Username);

                entity.Property(g => g.Username)
                      .HasMaxLength(256)
                      .IsRequired();

                entity.HasOne(p => p.Interest_Rate)
                      .WithMany(i => i.Personal_Savings_Accounts)
                      .HasForeignKey(p => p.Interest_Rate_ID)
                      .HasPrincipalKey(i => i.Interest_Rate_ID);
            });

            modelBuilder.Entity<Personal_Transactions_Information>(entity =>
            {
                entity.HasKey(p => p.Transaction_ID);

                entity.Property(p => p.Transaction_ID)
                      .ValueGeneratedOnAdd();

                entity.HasOne(p => p.Personal_Savings_Accounts)
                       .WithMany(p => p.Personal_Transactions_Information)
                       .HasForeignKey(p => p.Saving_ID)
                       .HasPrincipalKey(p => p.Saving_ID);
            });

            modelBuilder.Entity<Group_Savings_Accounts>(entity =>
            {
                entity.HasKey(p => p.Saving_ID);
                entity.Property(p => p.Saving_ID)
                      .ValueGeneratedOnAdd();

                entity.HasOne(p => p.Interest_Rates)
                      .WithMany(i => i.Group_Savings_Accounts)
                      .HasForeignKey(p => p.Interest_Rate_ID)
                      .HasPrincipalKey(i => i.Interest_Rate_ID);
            });

            modelBuilder.Entity<Group_Transactions_Information>(entity =>
            {
                entity.HasKey(p => p.Transaction_ID);
                entity.Property(p => p.Transaction_ID)
                      .ValueGeneratedOnAdd();

                entity.HasOne(p => p.Group_Savings_Accounts)
                      .WithMany(u => u.Group_Transactions_Information)
                      .HasForeignKey(p => p.Saving_ID)
                      .HasPrincipalKey(u => u.Saving_ID);

                entity.HasOne(p => p.User)
                      .WithMany(u => u.Group_Transactions_Information)
                      .HasForeignKey(p => p.Username)
                      .HasPrincipalKey(u => u.Username)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.Property(g => g.Username)
                      .HasMaxLength(256);
            });

            modelBuilder.Entity<Group_Details>(entity =>
            {
                entity.HasKey(g => new { g.Username, g.Saving_ID });

                entity.Property(g => g.Username)
                      .HasMaxLength(256)
                      .IsRequired();

                entity.HasOne(g => g.Group_Savings_Accounts)
                      .WithMany(gsa => gsa.Group_Details)
                      .HasForeignKey(g => g.Saving_ID)
                      .HasPrincipalKey(gsa => gsa.Saving_ID);

                entity.HasOne(g => g.User)
                      .WithMany(gsa => gsa.Group_Details)
                      .HasForeignKey(g => g.Username)
                      .HasPrincipalKey(gsa => gsa.Username)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Group_Notifications_Details>(entity =>
            {
                entity.HasKey(g => new { g.Username, g.Group_Notification_ID });

                entity.Property(g => g.Username)
                      .HasMaxLength(256)
                      .IsRequired();

                entity.HasOne(g => g.User)
                      .WithMany(gsa => gsa.Group_Notifications_Details)
                      .HasForeignKey(g => g.Username)
                      .HasPrincipalKey(gsa => gsa.Username);

                entity.HasOne(g => g.Group_Notifications)
                      .WithMany(gsa => gsa.Group_Notifications_Details)
                      .HasForeignKey(g => g.Group_Notification_ID)
                      .HasPrincipalKey(gsa => gsa.Group_Notification_ID);
            });

            modelBuilder.Entity<Personal_Notifications>(entity =>
            {
                entity.HasKey(g => g.Personal_Notification_ID);
                entity.Property(p => p.Personal_Notification_ID)
                      .ValueGeneratedOnAdd();

                entity.HasOne(g => g.Personal_Savings_Account)
                      .WithMany(gsa => gsa.Personal_Notifications)
                      .HasForeignKey(g => g.Saving_ID)
                      .HasPrincipalKey(gsa => gsa.Saving_ID);
            });

            modelBuilder.Entity<Group_Notifications>(entity =>
            {
                entity.HasKey(g => g.Group_Notification_ID);
                entity.Property(p => p.Group_Notification_ID)
                      .ValueGeneratedOnAdd();

                entity.HasOne(g => g.User_Sender)
                     .WithMany(u => u.Group_Notifications)
                       .HasForeignKey(g => g.Username_Sender)
                       .HasPrincipalKey(u => u.Username)
                        .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(g => g.Group_Savings_Accounts)
                     .WithMany(u => u.Group_Notifications)
                       .HasForeignKey(g => g.Saving_ID)
                       .HasPrincipalKey(u => u.Saving_ID);
            });

           base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=NgocAnh\\SQLEXPRESS;Database=QLSTK_DATABASE;" +
                "User ID=lengocanh123;Password=123456789;TrustServerCertificate=True;");
        }
    }
}
