
using GentleShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GentleShop.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            var passwordHasher = new PasswordHasher<User>();

            var users = new List<User>
            {
                new()
                {
                    Username = "admin",
                    FullName = "Nguyễn Quản Trị",
                    Email = "admin@gentleshop.vn",
                    Phone = "0900000001",
                    Address = "Hà Nội",
                    Role = 1,
                    Status = true,
                    CreatedAt = DateTime.Now
                },
                new()
                {
                    Username = "employee01",
                    FullName = "Nguyễn Văn An",
                    Email = "an@gentleshop.vn",
                    Phone = "0900000002",
                    Address = "Hà Nội",
                    Role = 2,
                    Status = true,
                    CreatedAt = DateTime.Now
                },
                new()
                {
                    Username = "employee02",
                    FullName = "Trần Thị Bình",
                    Email = "binh@gentleshop.vn",
                    Phone = "0900000003",
                    Address = "Hải Phòng",
                    Role = 2,
                    Status = true,
                    CreatedAt = DateTime.Now
                },
                new()
                {
                    Username = "customer01",
                    FullName = "Lê Văn Cường",
                    Email = "cuong@gmail.com",
                    Phone = "0900000004",
                    Address = "Hải Phòng",
                    Role = 3,
                    Status = true,
                    CreatedAt = DateTime.Now
                },
                new()
                {
                    Username = "customer02",
                    FullName = "Phạm Minh Đức",
                    Email = "duc@gmail.com",
                    Phone = "0900000005",
                    Address = "Hà Nội",
                    Role = 3,
                    Status = true,
                    CreatedAt = DateTime.Now
                },
                new()
                {
                    Username = "customer03",
                    FullName = "Hoàng Thị Hà",
                    Email = "ha@gmail.com",
                    Phone = "0900000006",
                    Address = "Quảng Ninh",
                    Role = 3,
                    Status = true,
                    CreatedAt = DateTime.Now
                }
            };

            foreach (var user in users)
            {
                bool exists = await context.Users.AnyAsync(u =>
                    u.Username == user.Username ||
                    u.Email == user.Email);

                if (!exists)
                {
                    user.Password = passwordHasher.HashPassword(
                        user,
                        "123456"
                    );

                    context.Users.Add(user);
                }
            }

            await context.SaveChangesAsync();


            var brands = new List<Brand>
            {
                new()
                {
                    Name = "Dell",
                    Description = "Thương hiệu máy tính nổi tiếng đến từ Hoa Kỳ."
                },
                new()
                {
                    Name = "HP",
                    Description = "Thương hiệu máy tính và thiết bị công nghệ của Hoa Kỳ."
                },
                new()
                {
                    Name = "ASUS",
                    Description = "Thương hiệu chuyên sản xuất laptop và linh kiện máy tính."
                },
                new()
                {
                    Name = "Lenovo",
                    Description = "Thương hiệu máy tính đến từ Trung Quốc."
                },
                new()
                {
                    Name = "Apple",
                    Description = "Thương hiệu công nghệ nổi tiếng với dòng MacBook."
                },
                new()
                {
                    Name = "Acer",
                    Description = "Thương hiệu máy tính và thiết bị điện tử."
                },
                new()
                {
                    Name = "MSI",
                    Description = "Thương hiệu chuyên về laptop gaming và thiết bị hiệu năng cao."
                }
            };

            foreach (var brand in brands)
            {
                bool exists = await context.Brands.AnyAsync(b =>
                    b.Name == brand.Name);

                if (!exists)
                {
                    context.Brands.Add(brand);
                }
            }

            await context.SaveChangesAsync();


            var categories = new List<Category>
            {
                new()
                {
                    Name = "Laptop văn phòng",
                    Description = "Laptop phục vụ học tập, làm việc văn phòng và nhu cầu cơ bản."
                },
                new()
                {
                    Name = "Laptop Gaming",
                    Description = "Laptop có cấu hình mạnh, chuyên phục vụ chơi game."
                },
                new()
                {
                    Name = "Laptop đồ họa",
                    Description = "Laptop phục vụ thiết kế đồ họa, dựng phim và xử lý hình ảnh."
                },
                new()
                {
                    Name = "Laptop mỏng nhẹ",
                    Description = "Laptop có thiết kế nhỏ gọn, trọng lượng nhẹ, dễ dàng di chuyển."
                },
                new()
                {
                    Name = "MacBook",
                    Description = "Các dòng máy tính xách tay MacBook của Apple."
                },
                new()
                {
                    Name = "Laptop doanh nghiệp",
                    Description = "Laptop phục vụ công việc chuyên nghiệp và doanh nghiệp."
                }
            };

            foreach (var category in categories)
            {
                bool exists = await context.Categories.AnyAsync(c =>
                    c.Name == category.Name);

                if (!exists)
                {
                    context.Categories.Add(category);
                }
            }

            await context.SaveChangesAsync();


            if (!await context.Products.AnyAsync())
            {
                var dell = await context.Brands
                    .FirstAsync(b => b.Name == "Dell");

                var hp = await context.Brands
                    .FirstAsync(b => b.Name == "HP");

                var asus = await context.Brands
                    .FirstAsync(b => b.Name == "ASUS");

                var lenovo = await context.Brands
                    .FirstAsync(b => b.Name == "Lenovo");

                var apple = await context.Brands
                    .FirstAsync(b => b.Name == "Apple");

                var msi = await context.Brands
                    .FirstAsync(b => b.Name == "MSI");

                var office = await context.Categories
                    .FirstAsync(c => c.Name == "Laptop văn phòng");

                var gaming = await context.Categories
                    .FirstAsync(c => c.Name == "Laptop Gaming");

                var graphics = await context.Categories
                    .FirstAsync(c => c.Name == "Laptop đồ họa");

                var lightweight = await context.Categories
                    .FirstAsync(c => c.Name == "Laptop mỏng nhẹ");

                var macbook = await context.Categories
                    .FirstAsync(c => c.Name == "MacBook");

                var business = await context.Categories
                    .FirstAsync(c => c.Name == "Laptop doanh nghiệp");

                var products = new List<Product>
                {
                    new()
                    {
                        Name = "Dell Inspiron 15 3530",
                        Slug = "dell-inspiron-15-3530",
                        ShortDescription = "Laptop văn phòng với hiệu năng ổn định.",
                        Description = "Dell Inspiron 15 3530 phù hợp cho học tập, làm việc văn phòng và các nhu cầu sử dụng hàng ngày.",
                        BrandId = dell.Id,
                        CategoryId = office.Id,
                        RatingAvg = 4.5f,
                        RatingCount = 28,
                        ViewCount = 1250,
                        SoldCount = 35,
                        IsFeatured = true,
                        IsNew = true,
                        Status = "active",
                        Specifications = "CPU: Intel Core i5\nRAM: 8GB\nSSD: 512GB\nMàn hình: 15.6 inch",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },

                    new()
                    {
                        Name = "HP Pavilion 15",
                        Slug = "hp-pavilion-15",
                        ShortDescription = "Laptop đa dụng cho học tập và công việc.",
                        Description = "HP Pavilion 15 sở hữu thiết kế hiện đại, đáp ứng tốt nhu cầu học tập và làm việc.",
                        BrandId = hp.Id,
                        CategoryId = office.Id,
                        RatingAvg = 4.3f,
                        RatingCount = 19,
                        ViewCount = 980,
                        SoldCount = 22,
                        IsFeatured = false,
                        IsNew = true,
                        Status = "active",
                        Specifications = "CPU: Intel Core i5\nRAM: 16GB\nSSD: 512GB\nMàn hình: 15.6 inch",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },

                    new()
                    {
                        Name = "ASUS TUF Gaming F15",
                        Slug = "asus-tuf-gaming-f15",
                        ShortDescription = "Laptop gaming hiệu năng cao.",
                        Description = "ASUS TUF Gaming F15 hướng đến người dùng chơi game và cần hiệu năng xử lý mạnh.",
                        BrandId = asus.Id,
                        CategoryId = gaming.Id,
                        RatingAvg = 4.8f,
                        RatingCount = 42,
                        ViewCount = 3200,
                        SoldCount = 58,
                        IsFeatured = true,
                        IsNew = false,
                        Status = "active",
                        Specifications = "CPU: Intel Core i7\nRAM: 16GB\nSSD: 512GB\nGPU: NVIDIA GeForce RTX",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },

                    new()
                    {
                        Name = "MSI Katana 15",
                        Slug = "msi-katana-15",
                        ShortDescription = "Laptop gaming dành cho người dùng hiệu năng cao.",
                        Description = "MSI Katana 15 được thiết kế dành cho gaming và các tác vụ đòi hỏi khả năng xử lý đồ họa.",
                        BrandId = msi.Id,
                        CategoryId = gaming.Id,
                        RatingAvg = 4.6f,
                        RatingCount = 31,
                        ViewCount = 2100,
                        SoldCount = 40,
                        IsFeatured = true,
                        IsNew = true,
                        Status = "active",
                        Specifications = "CPU: Intel Core i7\nRAM: 16GB\nSSD: 1TB\nGPU: NVIDIA GeForce RTX",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },

                    new()
                    {
                        Name = "Lenovo ThinkPad E14",
                        Slug = "lenovo-thinkpad-e14",
                        ShortDescription = "Laptop doanh nghiệp bền bỉ và ổn định.",
                        Description = "Lenovo ThinkPad E14 phù hợp với nhân viên văn phòng và người dùng doanh nghiệp.",
                        BrandId = lenovo.Id,
                        CategoryId = business.Id,
                        RatingAvg = 4.7f,
                        RatingCount = 36,
                        ViewCount = 1800,
                        SoldCount = 47,
                        IsFeatured = true,
                        IsNew = false,
                        Status = "active",
                        Specifications = "CPU: Intel Core i5\nRAM: 16GB\nSSD: 512GB\nMàn hình: 14 inch",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },

                    new()
                    {
                        Name = "MacBook Air M3",
                        Slug = "macbook-air-m3",
                        ShortDescription = "Laptop Apple mỏng nhẹ và có hiệu năng tốt.",
                        Description = "MacBook Air M3 có thiết kế mỏng nhẹ, phù hợp cho học tập, làm việc và sáng tạo nội dung.",
                        BrandId = apple.Id,
                        CategoryId = macbook.Id,
                        RatingAvg = 4.9f,
                        RatingCount = 55,
                        ViewCount = 4500,
                        SoldCount = 72,
                        IsFeatured = true,
                        IsNew = true,
                        Status = "active",
                        Specifications = "Chip: Apple M3\nRAM: 8GB\nSSD: 256GB\nMàn hình: Liquid Retina",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },

                    new()
                    {
                        Name = "ASUS Zenbook 14 OLED",
                        Slug = "asus-zenbook-14-oled",
                        ShortDescription = "Laptop OLED mỏng nhẹ dành cho công việc.",
                        Description = "ASUS Zenbook 14 OLED nổi bật với thiết kế nhỏ gọn và màn hình OLED chất lượng cao.",
                        BrandId = asus.Id,
                        CategoryId = lightweight.Id,
                        RatingAvg = 4.8f,
                        RatingCount = 24,
                        ViewCount = 1650,
                        SoldCount = 29,
                        IsFeatured = true,
                        IsNew = true,
                        Status = "active",
                        Specifications = "CPU: Intel Core Ultra 5\nRAM: 16GB\nSSD: 512GB\nMàn hình: 14 inch OLED",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },

                    new()
                    {
                        Name = "Dell XPS 15",
                        Slug = "dell-xps-15",
                        ShortDescription = "Laptop cao cấp dành cho thiết kế và đồ họa.",
                        Description = "Dell XPS 15 hướng đến người dùng chuyên nghiệp với nhu cầu thiết kế, lập trình và xử lý đa phương tiện.",
                        BrandId = dell.Id,
                        CategoryId = graphics.Id,
                        RatingAvg = 4.7f,
                        RatingCount = 33,
                        ViewCount = 2700,
                        SoldCount = 38,
                        IsFeatured = true,
                        IsNew = false,
                        Status = "active",
                        Specifications = "CPU: Intel Core i7\nRAM: 16GB\nSSD: 1TB\nGPU: NVIDIA GeForce RTX",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
