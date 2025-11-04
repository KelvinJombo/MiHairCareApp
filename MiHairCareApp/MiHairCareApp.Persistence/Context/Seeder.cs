using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Domain.Enums;
using MiHairCareApp.Persistence.Context;

namespace MiHairCareApp.Commons.Utilities
{
    public static class Seeder
    {
        public static async Task SeedRolesAndSuperAdmin(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<StylistsDBContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            // ===== Seed Roles =====
            string[] roles = { "Stylist", "User", "Admin" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // ===== Seed Super Admin =====
            if (await userManager.FindByNameAsync("Admin") == null)
            {
                var admin = new AppUser
                {
                    UserName = "Admin",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(admin, "Password@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // ===== Seed Haircare Products =====
            if (!context.HaircareProducts.Any())
            {
                var products = new List<HaircareProduct>
                {
                    // === Growth Products ===
                    new() { ProductName = "Herbal Growth Serum", Brand = "MiCare", Category = ProductCategory.Growth, Description = "Stimulates hair follicles for rapid growth", Price = 4500, ImageUrl = "https://res.cloudinary.com/seed/growth_serum.jpg", StockQuantity = 40 },
                    new() { ProductName = "Aloe Vera Growth Cream", Brand = "NaturGrow", Category = ProductCategory.Growth, Description = "Moisturizes scalp and supports new hair growth", Price = 4000, ImageUrl = "https://res.cloudinary.com/seed/aloe_growth_cream.jpg", StockQuantity = 50 },
                    new() { ProductName = "Coconut Hair Booster", Brand = "TropiCare", Category = ProductCategory.Growth, Description = "Nourishing blend for longer, thicker hair", Price = 3500, ImageUrl = "https://res.cloudinary.com/seed/coconut_booster.jpg", StockQuantity = 35 },
                    new() { ProductName = "Onion Hair Serum", Brand = "GlowRoots", Category = ProductCategory.Growth, Description = "Prevents hair fall and enhances root strength", Price = 3000, ImageUrl = "https://res.cloudinary.com/seed/onion_serum.jpg", StockQuantity = 30 },
                    new() { ProductName = "Avocado Growth Mask", Brand = "Hairly", Category = ProductCategory.Growth, Description = "Deep-nourishing mask for dull and weak hair", Price = 3800, ImageUrl = "https://res.cloudinary.com/seed/avocado_mask.jpg", StockQuantity = 45 },

                    // === Treatment Products ===
                    new() { ProductName = "Keratin Smooth Treatment", Brand = "SilkyLocks", Category = ProductCategory.Treatment, Description = "Repairs and strengthens damaged hair", Price = 6000, ImageUrl = "https://res.cloudinary.com/seed/keratin_smooth.jpg", StockQuantity = 25 },
                    new() { ProductName = "Argan Oil Repair Serum", Brand = "MorocSilk", Category = ProductCategory.Treatment, Description = "Revitalizes frizzy, dry hair", Price = 5500, ImageUrl = "https://res.cloudinary.com/seed/argan_serum.jpg", StockQuantity = 40 },
                    new() { ProductName = "Deep Nourish Mask", Brand = "Revive", Category = ProductCategory.Treatment, Description = "Hydrating mask for intense moisture", Price = 5000, ImageUrl = "https://res.cloudinary.com/seed/deep_mask.jpg", StockQuantity = 35 },
                    new() { ProductName = "Olive Oil Rebuilder", Brand = "GreenTouch", Category = ProductCategory.Treatment, Description = "Restores natural shine and resilience", Price = 4800, ImageUrl = "https://res.cloudinary.com/seed/olive_rebuilder.jpg", StockQuantity = 40 },
                    new() { ProductName = "Protein Reconstructor", Brand = "HairPro", Category = ProductCategory.Treatment, Description = "Repairs chemically treated hair", Price = 5200, ImageUrl = "https://res.cloudinary.com/seed/protein_reconstructor.jpg", StockQuantity = 30 },

                    // === Styling Products ===
                    new() { ProductName = "ShineMax Gel", Brand = "Glowly", Category = ProductCategory.Styling, Description = "Provides long-lasting hold and shine", Price = 2500, ImageUrl = "https://res.cloudinary.com/seed/styling_gel.jpg", StockQuantity = 60 },
                    new() { ProductName = "Hair Mousse", Brand = "SoftTouch", Category = ProductCategory.Styling, Description = "Volumizing mousse for flexible styles", Price = 2800, ImageUrl = "https://res.cloudinary.com/seed/mousse.jpg", StockQuantity = 55 },
                    new() { ProductName = "Edge Control Wax", Brand = "StyleIt", Category = ProductCategory.Styling, Description = "Perfect for edges and smooth finishes", Price = 3000, ImageUrl = "https://res.cloudinary.com/seed/edge_control.jpg", StockQuantity = 70 },
                    new() { ProductName = "Curl Enhancer Cream", Brand = "TwistPro", Category = ProductCategory.Styling, Description = "Defines curls and reduces frizz", Price = 3200, ImageUrl = "https://res.cloudinary.com/seed/curl_enhancer.jpg", StockQuantity = 50 },
                    new() { ProductName = "Heat Protection Spray", Brand = "ThermoShield", Category = ProductCategory.Styling, Description = "Protects hair from heat damage", Price = 3500, ImageUrl = "https://res.cloudinary.com/seed/heat_spray.jpg", StockQuantity = 45 },

                    // === Gadgets ===
                    new() { ProductName = "Hair Dryer Pro 3000", Brand = "SalonTech", Category = ProductCategory.Gadgets, Description = "Fast drying with heat control", Price = 12000, ImageUrl = "https://res.cloudinary.com/seed/hair_dryer.jpg", StockQuantity = 20 },
                    new() { ProductName = "Ceramic Straightener", Brand = "HeatWave", Category = ProductCategory.Gadgets, Description = "Smooths hair evenly with less damage", Price = 11000, ImageUrl = "https://res.cloudinary.com/seed/straightener.jpg", StockQuantity = 15 },
                    new() { ProductName = "Curling Wand Pro", Brand = "WaveMaker", Category = ProductCategory.Gadgets, Description = "Perfect curls with adjustable temperature", Price = 13000, ImageUrl = "https://res.cloudinary.com/seed/curling_wand.jpg", StockQuantity = 10 },
                    new() { ProductName = "Scalp Massager", Brand = "MiCare", Category = ProductCategory.Gadgets, Description = "Promotes relaxation and blood circulation", Price = 4500, ImageUrl = "https://res.cloudinary.com/seed/massager.jpg", StockQuantity = 25 },
                    new() { ProductName = "Hair Trimmer Kit", Brand = "EdgeMaster", Category = ProductCategory.Gadgets, Description = "Precision trimming tool", Price = 9500, ImageUrl = "https://res.cloudinary.com/seed/trimmer.jpg", StockQuantity = 12 },

                    // === Extensions ===
                    new() { ProductName = "Brazilian Weave 12-inch", Brand = "LuxStrands", Category = ProductCategory.Extensions, Description = "100% human hair weave", Price = 25000, ImageUrl = "https://res.cloudinary.com/seed/brazilian12.jpg", StockQuantity = 10 },
                    new() { ProductName = "Peruvian Weave 14-inch", Brand = "HairQueen", Category = ProductCategory.Extensions, Description = "Soft and durable texture", Price = 28000, ImageUrl = "https://res.cloudinary.com/seed/peruvian14.jpg", StockQuantity = 12 },
                    new() { ProductName = "Lace Wig 18-inch", Brand = "BeautyPro", Category = ProductCategory.Extensions, Description = "Pre-plucked frontal lace wig", Price = 32000, ImageUrl = "https://res.cloudinary.com/seed/lacewig18.jpg", StockQuantity = 8 },
                    new() { ProductName = "Clip-in Extensions", Brand = "StyleEase", Category = ProductCategory.Extensions, Description = "Easy to attach and remove", Price = 18000, ImageUrl = "https://res.cloudinary.com/seed/clipins.jpg", StockQuantity = 20 },
                    new() { ProductName = "Kinky Curls 16-inch", Brand = "CurlCrown", Category = ProductCategory.Extensions, Description = "Full and bouncy curls", Price = 27000, ImageUrl = "https://res.cloudinary.com/seed/kinky16.jpg", StockQuantity = 15 }
                };

                context.HaircareProducts.AddRange(products);
            }

            // ===== Seed Hair Styles =====
            if (!context.HairStyles.Any())
            {
                var hairstyles = new List<HairStyle>
                {
                    new() { StyleName = "Classic African Braids", Description = "Neat traditional braids suitable for all occasions", PriceTag = 15000, Origin = HairStyleOrigin.African, PromotionalOffer = false, Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/african_braids.jpg", IsMain = true } } },
                    new() { StyleName = "American Bob Cut", Description = "Trendy short cut with smooth finish", PriceTag = 18000, Origin = HairStyleOrigin.American, PromotionalOffer = true, Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/american_bob.jpg", IsMain = true } } },
                    new() { StyleName = "European Waves", Description = "Soft and relaxed waves with layered finish", PriceTag = 20000, Origin = HairStyleOrigin.European, PromotionalOffer = false, Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/european_waves.jpg", IsMain = true } } },
                    new() { StyleName = "Asian Pony Tail", Description = "Sleek ponytail inspired by Asian elegance", PriceTag = 16000, Origin = HairStyleOrigin.Asian, PromotionalOffer = true, Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/asian_ponytail.jpg", IsMain = true } } },
                    new() { StyleName = "Tribal Cornrows", Description = "Intricate African cornrow patterns", PriceTag = 17000, Origin = HairStyleOrigin.African, PromotionalOffer = false, Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/tribal_cornrows.jpg", IsMain = true } } }
                };

                context.HairStyles.AddRange(hairstyles);
            }

            await context.SaveChangesAsync();
        }
    }
}
