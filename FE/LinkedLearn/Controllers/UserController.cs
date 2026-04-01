using Microsoft.AspNetCore.Mvc;

namespace LinkedLearn.Controllers
{
    public class UserController : Controller
    {
        // Trang xem thông tin cá nhân
        public IActionResult Profile()
        {
            return View();
        }

        // Trang chỉnh sửa thông tin
        public IActionResult EditProfile()
        {
            return View();
        }
    }
}
