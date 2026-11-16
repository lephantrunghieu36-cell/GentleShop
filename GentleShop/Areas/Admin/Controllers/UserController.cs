using Classroom360.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Classroom360.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? role, string? search)
        {
            var query = _context.Users
                .AsNoTracking()
                .Where(u => u.Role == "EMPLOYEE" || u.Role == "CUSTOMER");

            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(u => u.Role == role);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.FullName.Contains(search) ||
                    u.Email.Contains(search));
            }

            ViewBag.Role = role;
            ViewBag.Search = search;

            var users = await query
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return View(users);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null ||
                (user.Role != "EMPLOYEE" && user.Role != "CUSTOMER"))
                return NotFound();

            return View(user);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new User
            {
                Role = "STUDENT",
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                ModelState.AddModelError("",
                    "Mật khẩu phải có ít nhất 6 ký tự!");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                ModelState.AddModelError(nameof(user.Email),
                    "Email không được để trống!");
            }
            else if (await _context.Users.AnyAsync(
                u => u.Email == user.Email.Trim()))
            {
                ModelState.AddModelError(nameof(user.Email),
                    "Email này đã được sử dụng!");
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            user.Email = user.Email.Trim();
            user.CreatedAt = DateTime.Now;

            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm người dùng thành công!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int id,
    string FullName,
    string Email,
    string Phone,
    string Role,
    bool IsActive)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                FullName = FullName?.Trim();
                Email = Email?.Trim();

                if (string.IsNullOrWhiteSpace(FullName))
                {
                    ModelState.AddModelError(
                        nameof(FullName),
                        "Họ tên không được để trống!");
                }

                if (string.IsNullOrWhiteSpace(Email))
                {
                    ModelState.AddModelError(
                        nameof(Email),
                        "Email không được để trống!");
                }
                else if (await _context.Users.AnyAsync(
                    u => u.Email == Email && u.UserId != id))
                {
                    ModelState.AddModelError(
                        nameof(Email),
                        "Email này đã được sử dụng!");
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value != null
                                 && x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors.Select(e => new
                        {
                            Field = x.Key,
                            Error = e.ErrorMessage,
                            Exception = e.Exception?.ToString()
                        }))
                        .ToList();

                    user.FullName = FullName ?? string.Empty;
                    user.Email = Email ?? string.Empty;
                    user.Phone = Phone;
                    user.Role = Role;

                    user.IsActive = IsActive;

                    return View(user);
                }

                user.FullName = FullName!;
                user.Email = Email!;
                user.Phone = Phone;
                user.Role = Role;
                user.IsActive = IsActive;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật người dùng thành công!";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, bool isActive)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {

                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy tài khoản!"
                    });
                }

                user.IsActive = isActive;

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Cập nhật trạng thái tài khoản thành công!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi cập nhật trạng thái tài khoản!"
                });
            }
        }
    }
}