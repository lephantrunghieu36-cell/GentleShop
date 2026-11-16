
using Microsoft.AspNetCore.Mvc;

namespace Classroom360.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "ADMIN")
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Auth", new { area = "" });
            }

            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            TempData["Success"] = "Bạn đã đăng xuất thành công!";

            return RedirectToAction("Login", "Auth", new { area = "" });
        }
    }
}
