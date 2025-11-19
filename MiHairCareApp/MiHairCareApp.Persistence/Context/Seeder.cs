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
            // ===== Seed Haircare Products =====
            if (!context.HaircareProducts.Any())
            {
                var products = new List<HaircareProduct>
                {
                        // === Growth Products ===
                        new() {
                            ProductName = "Herbal Growth Serum", Brand = "MiCare",
                            Category = ProductCategory.Growth,
                            Description = "Stimulates hair follicles for rapid growth",
                            Price = 4500, StockQuantity = 40,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/growth_serum.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Aloe Vera Growth Cream", Brand = "NaturGrow",
                            Category = ProductCategory.Growth,
                            Description = "Moisturizes scalp and supports new hair growth",
                            Price = 4000, StockQuantity = 50,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/aloe_growth_cream.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Coconut Hair Booster", Brand = "TropiCare",
                            Category = ProductCategory.Growth,
                            Description = "Nourishing blend for longer, thicker hair",
                            Price = 3500, StockQuantity = 35,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/coconut_booster.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Onion Hair Serum", Brand = "GlowRoots",
                            Category = ProductCategory.Growth,
                            Description = "Prevents hair fall and enhances root strength",
                            Price = 3000, StockQuantity = 30,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/onion_serum.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Avocado Growth Mask", Brand = "Hairly",
                            Category = ProductCategory.Growth,
                            Description = "Deep-nourishing mask for dull and weak hair",
                            Price = 3800, StockQuantity = 45,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/avocado_mask.jpg", IsMain = true } }
                        },

                        // === Treatment Products ===
                        new() {
                            ProductName = "Keratin Smooth Treatment", Brand = "SilkyLocks",
                            Category = ProductCategory.Treatment,
                            Description = "Repairs and strengthens damaged hair",
                            Price = 6000, StockQuantity = 25,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/keratin_smooth.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Argan Oil Repair Serum", Brand = "MorocSilk",
                            Category = ProductCategory.Treatment,
                            Description = "Revitalizes frizzy, dry hair",
                            Price = 5500, StockQuantity = 40,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/argan_serum.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Deep Nourish Mask", Brand = "Revive",
                            Category = ProductCategory.Treatment,
                            Description = "Hydrating mask for intense moisture",
                            Price = 5000, StockQuantity = 35,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/deep_mask.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Olive Oil Rebuilder", Brand = "GreenTouch",
                            Category = ProductCategory.Treatment,
                            Description = "Restores natural shine and resilience",
                            Price = 4800, StockQuantity = 40,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/olive_rebuilder.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Protein Reconstructor", Brand = "HairPro",
                            Category = ProductCategory.Treatment,
                            Description = "Repairs chemically treated hair",
                            Price = 5200, StockQuantity = 30,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/protein_reconstructor.jpg", IsMain = true } }
                        },

                        // === Styling Products ===
                        new() {
                            ProductName = "ShineMax Gel", Brand = "Glowly",
                            Category = ProductCategory.Styling,
                            Description = "Provides long-lasting hold and shine",
                            Price = 2500, StockQuantity = 60,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/styling_gel.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Hair Mousse", Brand = "SoftTouch",
                            Category = ProductCategory.Styling,
                            Description = "Volumizing mousse for flexible styles",
                            Price = 2800, StockQuantity = 55,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/mousse.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Edge Control Wax", Brand = "StyleIt",
                            Category = ProductCategory.Styling,
                            Description = "Perfect for edges and smooth finishes",
                            Price = 3000, StockQuantity = 70,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/edge_control.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Curl Enhancer Cream", Brand = "TwistPro",
                            Category = ProductCategory.Styling,
                            Description = "Defines curls and reduces frizz",
                            Price = 3200, StockQuantity = 50,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/curl_enhancer.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Heat Protection Spray", Brand = "ThermoShield",
                            Category = ProductCategory.Styling,
                            Description = "Protects hair from heat damage",
                            Price = 3500, StockQuantity = 45,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/heat_spray.jpg", IsMain = true } }
                        },

                        // === Gadgets ===
                        new() {
                            ProductName = "Hair Dryer Pro 3000", Brand = "SalonTech",
                            Category = ProductCategory.Gadgets,
                            Description = "Fast drying with heat control",
                            Price = 12000, StockQuantity = 20,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/hair_dryer.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Ceramic Straightener", Brand = "HeatWave",
                            Category = ProductCategory.Gadgets,
                            Description = "Smooths hair evenly with less damage",
                            Price = 11000, StockQuantity = 15,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/straightener.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Curling Wand Pro", Brand = "WaveMaker",
                            Category = ProductCategory.Gadgets,
                            Description = "Perfect curls with adjustable temperature",
                            Price = 13000, StockQuantity = 10,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/curling_wand.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Scalp Massager", Brand = "MiCare",
                            Category = ProductCategory.Gadgets,
                            Description = "Promotes relaxation and blood circulation",
                            Price = 4500, StockQuantity = 25,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/massager.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Hair Trimmer Kit", Brand = "EdgeMaster",
                            Category = ProductCategory.Gadgets,
                            Description = "Precision trimming tool",
                            Price = 9500, StockQuantity = 12,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/trimmer.jpg", IsMain = true } }
                        },

                        // === Extensions ===
                        new() {
                            ProductName = "Brazilian Weave 12-inch", Brand = "LuxStrands",
                            Category = ProductCategory.Extensions,
                            Description = "100% human hair weave",
                            Price = 25000, StockQuantity = 10,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/brazilian12.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Peruvian Weave 14-inch", Brand = "HairQueen",
                            Category = ProductCategory.Extensions,
                            Description = "Soft and durable texture",
                            Price = 28000, StockQuantity = 12,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/peruvian14.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Lace Wig 18-inch", Brand = "BeautyPro",
                            Category = ProductCategory.Extensions,
                            Description = "Pre-plucked frontal lace wig",
                            Price = 32000, StockQuantity = 8,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/lacewig18.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Clip-in Extensions", Brand = "StyleEase",
                            Category = ProductCategory.Extensions,
                            Description = "Easy to attach and remove",
                            Price = 18000, StockQuantity = 20,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/clipins.jpg", IsMain = true } }
                        },

                        new() {
                            ProductName = "Kinky Curls 16-inch", Brand = "CurlCrown",
                            Category = ProductCategory.Extensions,
                            Description = "Full and bouncy curls",
                            Price = 27000, StockQuantity = 15,
                            Photos = new List<Photo>{ new() { Url = "https://res.cloudinary.com/seed/kinky16.jpg", IsMain = true } }
                        },
                };


                context.HaircareProducts.AddRange(products);
            }
        }
    }
}
