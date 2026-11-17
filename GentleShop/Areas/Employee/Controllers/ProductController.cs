using GentleShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace GentleShop.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        private const long MaxImageSize = 5 * 1024 * 1024;

        private static readonly string[] AllowedExtensions =
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        public ProductController(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string? search, string? status)
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    (p.Slug != null && p.Slug.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(p => p.Status == status);
            }

            ViewBag.Search = search;
            ViewBag.Status = status;

            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.ProductImages = await _context.ProductImages
                .AsNoTracking()
                .Where(i => i.ProductId == id)
                .ToListAsync();

            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Product product,
            IFormFile? thumbnailFile,
            List<IFormFile>? imageFiles)
        {
            ModelState.Remove(nameof(Product.Brand));
            ModelState.Remove(nameof(Product.Category));

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                ModelState.AddModelError(
                    nameof(product.Name),
                    "Vui lòng nhập tên sản phẩm.");
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(product.BrandId, product.CategoryId);
                return View(product);
            }

            if (!await ValidateRelationsAsync(product.BrandId, product.CategoryId))
            {
                await LoadDropdownsAsync(product.BrandId, product.CategoryId);
                return View(product);
            }

            product.Name = product.Name.Trim();
            product.Slug = await GenerateUniqueSlugAsync(product.Name);
            product.Status = NormalizeStatus(product.Status);
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;

            if (thumbnailFile != null && thumbnailFile.Length > 0)
            {
                var error = ValidateImage(thumbnailFile);

                if (error != null)
                {
                    ModelState.AddModelError(nameof(thumbnailFile), error);
                    await LoadDropdownsAsync(product.BrandId, product.CategoryId);
                    return View(product);
                }

                product.Thumbnail = await SaveImageAsync(thumbnailFile);
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            if (imageFiles != null)
            {
                foreach (var file in imageFiles.Where(f => f != null && f.Length > 0))
                {
                    var error = ValidateImage(file);

                    if (error != null)
                    {
                        TempData["Warning"] = $"Một số ảnh không được tải lên: {error}";
                        continue;
                    }

                    var imageUrl = await SaveImageAsync(file);

                    _context.ProductImages.Add(new ProductImage
                    {
                        ProductId = product.Id,
                        Url = imageUrl
                    });
                }

                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Thêm sản phẩm thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.ProductImages = await _context.ProductImages
                .Where(i => i.ProductId == id)
                .ToListAsync();

            await LoadDropdownsAsync(product.BrandId, product.CategoryId);

            return View(product);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Product model,
            IFormFile? thumbnailFile,
            List<IFormFile>? imageFiles)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            ModelState.Remove(nameof(Product.Brand));
            ModelState.Remove(nameof(Product.Category));

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "Vui lòng nhập tên sản phẩm.");
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ProductImages = await _context.ProductImages
                    .Where(i => i.ProductId == id)
                    .ToListAsync();

                await LoadDropdownsAsync(model.BrandId, model.CategoryId);
                return View(model);
            }

            if (!await ValidateRelationsAsync(model.BrandId, model.CategoryId))
            {
                ViewBag.ProductImages = await _context.ProductImages
                    .Where(i => i.ProductId == id)
                    .ToListAsync();

                await LoadDropdownsAsync(model.BrandId, model.CategoryId);
                return View(model);
            }

            var newName = model.Name.Trim();

            product.Name = newName;
            product.Slug = await GenerateUniqueSlugAsync(newName, id);
            product.ShortDescription = model.ShortDescription;
            product.Description = model.Description;
            product.BrandId = model.BrandId;
            product.CategoryId = model.CategoryId;
            product.RatingAvg = model.RatingAvg;
            product.RatingCount = model.RatingCount;
            product.ViewCount = model.ViewCount;
            product.SoldCount = model.SoldCount;
            product.IsFeatured = model.IsFeatured;
            product.IsNew = model.IsNew;
            product.Status = NormalizeStatus(model.Status);
            product.Specifications = model.Specifications;
            product.UpdatedAt = DateTime.Now;

            if (thumbnailFile != null && thumbnailFile.Length > 0)
            {
                var error = ValidateImage(thumbnailFile);

                if (error != null)
                {
                    ModelState.AddModelError(nameof(thumbnailFile), error);

                    ViewBag.ProductImages = await _context.ProductImages
                        .Where(i => i.ProductId == id)
                        .ToListAsync();

                    await LoadDropdownsAsync(model.BrandId, model.CategoryId);
                    return View(model);
                }

                var oldThumbnail = product.Thumbnail;
                product.Thumbnail = await SaveImageAsync(thumbnailFile);
                DeleteImageFile(oldThumbnail);
            }

            if (imageFiles != null)
            {
                foreach (var file in imageFiles.Where(f => f != null && f.Length > 0))
                {
                    var error = ValidateImage(file);

                    if (error != null)
                    {
                        TempData["Warning"] = $"Một số ảnh không được tải lên: {error}";
                        continue;
                    }

                    var imageUrl = await SaveImageAsync(file);

                    _context.ProductImages.Add(new ProductImage
                    {
                        ProductId = product.Id,
                        Url = imageUrl
                    });
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật sản phẩm thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.ProductImages
                .FirstOrDefaultAsync(i => i.Id == id);

            if (image == null)
            {
                return NotFound();
            }

            var productId = image.ProductId;

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();

            DeleteImageFile(image.Url);

            TempData["Success"] = "Đã xóa ảnh sản phẩm.";

            return RedirectToAction(nameof(Edit), new { id = productId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var productImages = await _context.ProductImages
                .Where(i => i.ProductId == id)
                .ToListAsync();

            _context.ProductImages.RemoveRange(productImages);
            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            DeleteImageFile(product.Thumbnail);

            foreach (var image in productImages)
            {
                DeleteImageFile(image.Url);
            }

            TempData["Success"] = "Xóa sản phẩm thành công.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdownsAsync(
            int? selectedBrandId = null,
            int? selectedCategoryId = null)
        {
            ViewBag.Brands = new SelectList(
                await _context.Brands
                    .AsNoTracking()
                    .OrderBy(b => b.Name)
                    .ToListAsync(),
                "Id",
                "Name",
                selectedBrandId
            );

            ViewBag.Categories = new SelectList(
                await _context.Categories
                    .AsNoTracking()
                    .OrderBy(c => c.Name)
                    .ToListAsync(),
                "Id",
                "Name",
                selectedCategoryId
            );
        }

        private async Task<bool> ValidateRelationsAsync(
            int? brandId,
            int? categoryId)
        {
            if (brandId.HasValue &&
                !await _context.Brands.AnyAsync(b => b.Id == brandId.Value))
            {
                ModelState.AddModelError(
                    nameof(Product.BrandId),
                    "Thương hiệu không tồn tại.");

                return false;
            }

            if (categoryId.HasValue &&
                !await _context.Categories.AnyAsync(c => c.Id == categoryId.Value))
            {
                ModelState.AddModelError(
                    nameof(Product.CategoryId),
                    "Danh mục không tồn tại.");

                return false;
            }

            return true;
        }

        private async Task<string> GenerateUniqueSlugAsync(
            string name,
            int? currentProductId = null)
        {
            var slug = CreateSlug(name);

            if (string.IsNullOrWhiteSpace(slug))
            {
                slug = "san-pham";
            }

            var result = slug;
            var counter = 1;

            while (await _context.Products.AnyAsync(p =>
                p.Slug == result &&
                (!currentProductId.HasValue || p.Id != currentProductId.Value)))
            {
                result = $"{slug}-{counter}";
                counter++;
            }

            return result;
        }

        private static string CreateSlug(string value)
        {
            value = value.Trim().ToLowerInvariant()
                .Replace("đ", "d");

            var normalized = value.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var character in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character)
                    != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(character);
                }
            }

            var result = builder.ToString()
                .Normalize(NormalizationForm.FormC);

            result = Regex.Replace(result, @"[^a-z0-9]+", "-");
            result = Regex.Replace(result, @"^-|-$", "");

            return result;
        }

        private string? ValidateImage(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
            {
                return "Chỉ chấp nhận ảnh JPG, JPEG, PNG hoặc WEBP.";
            }

            if (file.Length > MaxImageSize)
            {
                return "Dung lượng ảnh không được vượt quá 5MB.";
            }

            return null;
        }

        private async Task<string> SaveImageAsync(IFormFile file)
        {
            var webRoot = _environment.WebRootPath;

            if (string.IsNullOrWhiteSpace(webRoot))
            {
                webRoot = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            var uploadFolder = Path.Combine(
                webRoot,
                "uploads",
                "products"
            );

            Directory.CreateDirectory(uploadFolder);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadFolder, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/products/{fileName}";
        }

        private void DeleteImageFile(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return;
            }

            var fileName = Path.GetFileName(imageUrl);

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            var webRoot = _environment.WebRootPath;

            if (string.IsNullOrWhiteSpace(webRoot))
            {
                webRoot = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            var filePath = Path.Combine(
                webRoot,
                "uploads",
                "products",
                fileName
            );

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        private static string NormalizeStatus(string? status)
        {
            return string.Equals(
                status?.Trim(),
                "inactive",
                StringComparison.OrdinalIgnoreCase)
                ? "inactive"
                : "active";
        }
    }
}