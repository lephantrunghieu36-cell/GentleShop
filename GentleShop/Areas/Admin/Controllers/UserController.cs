
using GentleShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GentleShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new();

        private const int AdminRole = 1;
        private const int EmployeeRole = 2;
        private const int CustomerRole = 3;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? role, string? search)
        {
            var query = _context.Users
                .AsNoTracking()
                .Where(u => u.Role == EmployeeRole ||
                            u.Role == CustomerRole);

            if (role.HasValue)
            {
                query = query.Where(u => u.Role == role.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(u =>
                    u.FullName.Contains(search) ||
                    u.Email.Contains(search) ||
                    u.Username.Contains(search));
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
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null ||
                (user.Role != EmployeeRole &&
                 user.Role != CustomerRole))
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new User
            {
                Role = CustomerRole,
                Status = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password) ||
                password.Length < 6)
            {
                ModelState.AddModelError(
                    "password",
                    "Mật khẩu phải có ít nhất 6 ký tự!");
            }

            if (string.IsNullOrWhiteSpace(user.Username))
            {
                ModelState.AddModelError(
                    nameof(user.Username),
                    "Tên đăng nhập không được để trống!");
            }
            else if (await _context.Users.AnyAsync(
                u => u.Username == user.Username.Trim()))
            {
                ModelState.AddModelError(
                    nameof(user.Username),
                    "Tên đăng nhập đã tồn tại!");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                ModelState.AddModelError(
                    nameof(user.Email),
                    "Email không được để trống!");
            }
            else if (await _context.Users.AnyAsync(
                u => u.Email == user.Email.Trim()))
            {
                ModelState.AddModelError(
                    nameof(user.Email),
                    "Email này đã được sử dụng!");
            }

            if (user.Role != EmployeeRole &&
                user.Role != CustomerRole)
            {
                ModelState.AddModelError(
                    nameof(user.Role),
                    "Vai trò không hợp lệ!");
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            user.Username = user.Username.Trim();
            user.FullName = user.FullName.Trim();
            user.Email = user.Email.Trim();
            user.CreatedAt = DateTime.Now;

            user.Password = _passwordHasher.HashPassword(
                user, password);

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
                return NotFound();

            if (user.Role == AdminRole)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            string FullName,
            string Email,
            string? Username,
            string? Phone,
            int Role,
            bool Status)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            if (user.Role == AdminRole)
                return NotFound();

            FullName = FullName?.Trim() ?? "";
            Email = Email?.Trim() ?? "";
            Username = Username?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(FullName))
            {
                ModelState.AddModelError(
                    nameof(FullName),
                    "Họ tên không được để trống!");
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                ModelState.AddModelError(
                    nameof(Username),
                    "Tên đăng nhập không được để trống!");
            }
            else if (await _context.Users.AnyAsync(
                u => u.Username == Username && u.Id != id))
            {
                ModelState.AddModelError(
                    nameof(Username),
                    "Tên đăng nhập đã tồn tại!");
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError(
                    nameof(Email),
                    "Email không được để trống!");
            }
            else if (await _context.Users.AnyAsync(
                u => u.Email == Email && u.Id != id))
            {
                ModelState.AddModelError(
                    nameof(Email),
                    "Email này đã được sử dụng!");
            }

            if (Role != EmployeeRole &&
                Role != CustomerRole)
            {
                ModelState.AddModelError(
                    nameof(Role),
                    "Vai trò không hợp lệ!");
            }

            if (!ModelState.IsValid)
            {
                user.FullName = FullName;
                user.Email = Email;
                user.Username = Username;
                user.Phone = Phone;
                user.Role = Role;
                user.Status = Status;

                return View(user);
            }

            user.FullName = FullName;
            user.Email = Email;
            user.Username = Username;
            user.Phone = Phone;
            user.Role = Role;
            user.Status = Status;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật người dùng thành công!";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int id,
            bool status)
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

            if (user.Role == AdminRole)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Không thể thay đổi trạng thái tài khoản Admin!"
                });
            }

            user.Status = status;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Cập nhật trạng thái tài khoản thành công!"
            });
        }
    }
}
