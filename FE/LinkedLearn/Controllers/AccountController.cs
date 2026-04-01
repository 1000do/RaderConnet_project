// Đảm bảo đúng namespace của ViewModel
// Đảm bảo đúng namespace của Service
using LinkedLearn.Models;
using Microsoft.AspNetCore.Mvc;
using LinkedLearn.Service.IService;
using LinkedLearn.Models.UserVM;

namespace LinkedLearn.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserGatewayFrontendService _gatewayService;

        public AccountController(IUserGatewayFrontendService gatewayService)
        {
            _gatewayService = gatewayService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Gọi sang Gateway thay vì check cứng
            var (token, statusCode) = await _gatewayService.LoginAsync(model);

            if (statusCode == 200)
            {
                // 1. Lưu Token vào Cookie để các request sau đính kèm vào Header
                Response.Cookies.Append("JWTToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddHours(2)
                });

                // 2. Lưu tên vào Session để hiển thị trên giao diện Layout
                // (Bạn có thể parse Token để lấy tên thật, ở đây tạm set theo email)
                HttpContext.Session.SetString("UserName", model.Email);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Sai tài khoản hoặc mật khẩu rồi bồ tèo!";
            return View(model);
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (content, statusCode) = await _gatewayService.RegisterAsync(model);

            if (statusCode == 200 || statusCode == 201)
            {
                TempData["SuccessMessage"] = "Đăng ký ngon lành! Đăng nhập đi bạn.";
                return RedirectToAction("Login");
            }

            ViewBag.ErrorMessage = "Đăng ký lỗi rồi: " + content;
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("JWTToken"); // Xóa luôn cả Token
            return RedirectToAction("Login");
        }

        public IActionResult ForgotPassword() => View();
    }
}