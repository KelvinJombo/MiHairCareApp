using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiHairCareApp.Domain.Entities;
using System.Collections.Generic;

namespace MiHairCareApp.Persistence.Context
{
    public class StylistsDBContext : IdentityDbContext<IdentityUser>
    {
        public StylistsDBContext(DbContextOptions<StylistsDBContext> options)
            : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<HairStyle> HairStyles { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PromotionalOffers> PromotionalOffers { get; set; }
        public DbSet<Ratings> Ratings { get; set; }
        public DbSet<Referral> Referrals { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<StylistPortfolio> StylistPortfolios { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletFunding> WalletFundings { get; set; }
        public DbSet<UserTransaction> UserTransactionsSavings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure IdentityUser discriminator
            modelBuilder.Entity<IdentityUser>()
                .ToTable("AppUsers")
                .HasDiscriminator<string>("UserType")
                .HasValue<AppUser>("AppUser")
                .HasValue<Stylist>("Stylist");

            // Additional configurations for AppUser
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.Property(u => u.Id).ValueGeneratedOnAdd();
                entity.Property(u => u.FirstName).HasMaxLength(50);
                entity.Property(u => u.Password).HasMaxLength(100);
                entity.Property(u => u.Address).HasMaxLength(100);
                entity.Property(u => u.PasswordResetToken).HasMaxLength(100);
                entity.Property(u => u.ImageUrl).HasMaxLength(255);
            });

            // Additional configurations for Stylist
            modelBuilder.Entity<Stylist>(entity =>
            {
                entity.Property(s => s.Id).ValueGeneratedOnAdd();
                entity.Property(s => s.StylistName).HasMaxLength(50);
                entity.Property(s => s.CompanyName).HasMaxLength(100);
                entity.Property(s => s.Email).HasMaxLength(256);
                entity.Property(s => s.PasswordResetToken).HasMaxLength(100);
                entity.Property(s => s.Address).HasMaxLength(100);
                entity.Property(s => s.Town).HasMaxLength(50);
                entity.Property(s => s.Street).HasMaxLength(50);
                entity.Property(s => s.ImageUrl).HasMaxLength(255);
                entity.Property(s => s.BookingLink).HasMaxLength(100);
                entity.Property(s => s.PhoneNumber).HasMaxLength(20);
            });

            // Configure Referral foreign key relationships
            modelBuilder.Entity<Referral>(entity =>
            {
                entity.HasOne(r => r.ReferrerUser)
                    .WithMany(u => u.ReferralsMade)
                    .HasForeignKey(r => r.ReferrerUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Stylist)
                    .WithMany()
                    .HasForeignKey(r => r.StylistID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Booking foreign key relationships
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasOne(b => b.AppUser)
                    .WithMany(u => u.Bookings)
                    .HasForeignKey(b => b.AppUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Stylist)
                    .WithMany(s => s.Bookings)
                    .HasForeignKey(b => b.StylistID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.HairStyle)
                    .WithMany(h => h.Bookings)
                    .HasForeignKey(b => b.HairStyleID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Referral)
                    .WithMany(r => r.Bookings)
                    .HasForeignKey(b => b.ReferralID)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }


    }



}
