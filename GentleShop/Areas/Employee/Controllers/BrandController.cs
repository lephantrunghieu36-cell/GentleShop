
using GentleShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GentleShop.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        private readonly string[] _allowedExtensions =
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        private const long MaxFileSize = 5 * 1024 * 1024;

        public BrandController(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Brands.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(b =>
                    b.Name.Contains(search));
            }

            ViewBag.Search = search;

            var brands = await query
                .OrderBy(b => b.Name)
                .ToListAsync();

            return View(brands);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var brand = await _context.Brands
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (brand == null)
                return NotFound();

            return View(brand);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Brand());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Brand brand,
            IFormFile? imageFile)
        {
            if (string.IsNullOrWhiteSpace(brand.Name))
            {
                ModelState.AddModelError(
                    nameof(brand.Name),
                    "Tên thương hiệu không được để trống!");
            }
            else
            {
                brand.Name = brand.Name.Trim();

                if (await _context.Brands.AnyAsync(
                    b => b.Name == brand.Name))
                {
                    ModelState.AddModelError(
                        nameof(brand.Name),
                        "Tên thương hiệu đã tồn tại!");
                }
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                if (!ValidateImage(imageFile))
                {
                    ModelState.AddModelError(
                        "imageFile",
                        "Ảnh không hợp lệ. Chỉ chấp nhận JPG, JPEG, PNG, WEBP và tối đa 5MB!");
                }
            }

            if (!ModelState.IsValid)
                return View(brand);

            if (imageFile != null && imageFile.Length > 0)
            {
                brand.Img = await UploadImage(imageFile);
            }

            _context.Brands.Add(brand);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm thương hiệu thành công!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
                return NotFound();

            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Brand brand,
            IFormFile? imageFile)
        {
            var existingBrand = await _context.Brands
                .FirstOrDefaultAsync(b => b.Id == id);

            if (existingBrand == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(brand.Name))
            {
                ModelState.AddModelError(
                    nameof(brand.Name),
                    "Tên thương hiệu không được để trống!");
            }
            else
            {
                brand.Name = brand.Name.Trim();

                if (await _context.Brands.AnyAsync(
                    b => b.Name == brand.Name && b.Id != id))
                {
                    ModelState.AddModelError(
                        nameof(brand.Name),
                        "Tên thương hiệu đã tồn tại!");
                }
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                if (!ValidateImage(imageFile))
                {
                    ModelState.AddModelError(
                        "imageFile",
                        "Ảnh không hợp lệ. Chỉ chấp nhận JPG, JPEG, PNG, WEBP và tối đa 5MB!");
                }
            }

            if (!ModelState.IsValid)
            {
                brand.Id = id;
                brand.Img = existingBrand.Img;

                return View(brand);
            }

            existingBrand.Name = brand.Name;
            existingBrand.Description = brand.Description;

            if (imageFile != null && imageFile.Length > 0)
            {
                string? oldImage = existingBrand.Img;

                existingBrand.Img = await UploadImage(imageFile);

                DeleteImage(oldImage);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật thương hiệu thành công!";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                TempData["Error"] = "Không tìm thấy thương hiệu!";
                return RedirectToAction(nameof(Index));
            }

            string? image = brand.Img;

            _context.Brands.Remove(brand);

            await _context.SaveChangesAsync();

            DeleteImage(image);

            TempData["Success"] = "Xóa thương hiệu thành công!";

            return RedirectToAction(nameof(Index));
        }

        private bool ValidateImage(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName)
                .ToLowerInvariant();

            return file.Length <= MaxFileSize &&
                   _allowedExtensions.Contains(extension);
        }

        private async Task<string> UploadImage(IFormFile file)
        {
            string folder = Path.Combine(
                _environment.WebRootPath,
                "img");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string extension = Path.GetExtension(file.FileName)
                .ToLowerInvariant();

            string fileName = $"brand_{Guid.NewGuid():N}{extension}";

            string filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(
                filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/img/" + fileName;
        }

        private void DeleteImage(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return;

            string fileName = Path.GetFileName(imagePath);

            string fullPath = Path.Combine(
                _environment.WebRootPath,
                "img",
                fileName);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}
