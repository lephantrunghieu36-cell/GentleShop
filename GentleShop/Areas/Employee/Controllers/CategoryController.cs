
using GentleShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GentleShop.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Categories
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(c =>
                    c.Name.Contains(search));
            }

            ViewBag.Search = search;

            var categories = await query
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                ModelState.AddModelError(
                    nameof(category.Name),
                    "Tên danh mục không được để trống!");
            }
            else
            {
                category.Name = category.Name.Trim();

                if (await _context.Categories.AnyAsync(
                    c => c.Name == category.Name))
                {
                    ModelState.AddModelError(
                        nameof(category.Name),
                        "Tên danh mục đã tồn tại!");
                }
            }

            if (!ModelState.IsValid)
                return View(category);

            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm danh mục thành công!";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Category category)
        {
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCategory == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                ModelState.AddModelError(
                    nameof(category.Name),
                    "Tên danh mục không được để trống!");
            }
            else
            {
                category.Name = category.Name.Trim();

                if (await _context.Categories.AnyAsync(
                    c => c.Name == category.Name && c.Id != id))
                {
                    ModelState.AddModelError(
                        nameof(category.Name),
                        "Tên danh mục đã tồn tại!");
                }
            }

            if (!ModelState.IsValid)
            {
                category.Id = id;
                return View(category);
            }

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật danh mục thành công!";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                TempData["Error"] = "Không tìm thấy danh mục!";
                return RedirectToAction(nameof(Index));
            }

            bool hasProducts = await _context.Products
                .AnyAsync(p => p.CategoryId == id);

            if (hasProducts)
            {
                TempData["Error"] =
                    "Không thể xóa danh mục vì đang có sản phẩm sử dụng!";

                return RedirectToAction(nameof(Index));
            }

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa danh mục thành công!";

            return RedirectToAction(nameof(Index));
        }
    }
}
