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
        public DbSet<UserTransaction> UserTransactions { get; set; }
        public DbSet<HaircareProduct> HaircareProducts { get; set; }       
        //public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<Photo> Photos { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship between AppUser and Referral
            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.ReferralsMade)
                .WithOne(r => r.ReferrerUser)
                .HasForeignKey(r => r.ReferrerUserId)
                .OnDelete(DeleteBehavior.Restrict);             

            //modelBuilder.Entity<AppUser>()
            //    .HasMany(u => u.ProductReviews)
            //    .WithOne(r => r.User)
            //    .HasForeignKey(r => r.UserID)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.Review)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure other entities similarly to avoid cycles
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasOne(b => b.User)
                    .WithMany(u => u.Bookings)
                    .HasForeignKey(b => b.AppUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.HairStyle)
                    .WithMany(h => h.Bookings)
                    .HasForeignKey(b => b.HairStyleID)
                    .OnDelete(DeleteBehavior.Restrict);

                if (entity.HasOne(b => b.Referral) != null)
                {
                    entity.HasOne(b => b.Referral)
                        .WithMany(r => r.Bookings)
                        .HasForeignKey(b => b.ReferralID)
                        .OnDelete(DeleteBehavior.Restrict);
                }
            });
             
            modelBuilder.Entity<HairStyle>(entity =>
            {
                entity.HasMany(h => h.Bookings)
                    .WithOne(b => b.HairStyle)
                    .HasForeignKey(b => b.HairStyleID)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }

     

}
