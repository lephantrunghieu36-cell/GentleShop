
using Microsoft.AspNetCore.Mvc;

namespace GentleShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
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
