using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiHairCareApp.Domain.Entities;

namespace MiHairCareApp.Persistence.Context
{
    public class StylistsDBContext : IdentityDbContext<AppUser>
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
        public DbSet<Review> Reviews { get; set; }
        public DbSet<StylistPortfolio> StylistPortfolios { get; set; }
        public DbSet<Wallet> Wallets { get; set; }         
        public DbSet<Cart> Carts { get; set; }     
        public DbSet<SalesRecord> SalesRecords { get; set; }
        public DbSet<UserTransaction> UserTransactions { get; set; }
        public DbSet<HaircareProduct> HaircareProducts { get; set; }       
        public DbSet<Order> Orders { get; set; }
        public DbSet<Photo> Photos { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Referral Configurations
            modelBuilder.Entity<Referral>()
                .HasOne(r => r.ReferrerUser)
                .WithMany(u => u.ReferralsMade)
                .HasForeignKey(r => r.ReferrerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Referral>()
                .HasOne(r => r.ReferredUser)
                .WithMany(u => u.ReferralsReceived)
                .HasForeignKey(r => r.ReferredUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Referral>()
                .HasOne(r => r.Stylist)
                .WithMany() // No inverse navigation
                .HasForeignKey(r => r.StylistID)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking Configurations
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.HairStyle)
                .WithMany(h => h.Bookings)
                .HasForeignKey(b => b.HairStyleID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                 .HasOne(b => b.Referral) 
                 .WithMany(r => r.Bookings)
                 .HasForeignKey(b => b.ReferralID)
                 .OnDelete(DeleteBehavior.Restrict);


            // HairStyle Configurations
            modelBuilder.Entity<HairStyle>()
                .HasMany(h => h.Bookings)
                .WithOne(b => b.HairStyle)
                .HasForeignKey(b => b.HairStyleID)
                .OnDelete(DeleteBehavior.Restrict);

            // Review Configurations
            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.Review)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Review)
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Photo>()
                 .HasOne(p => p.HairStyle)
                 .WithMany(h => h.Photos)
                 .HasForeignKey(p => p.HairStyleId)
                 .OnDelete(DeleteBehavior.Cascade);


        }

    }



}
