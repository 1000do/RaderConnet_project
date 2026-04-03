using LinkedLearn.Models;
using Microsoft.AspNetCore.Mvc;
using LinkedLearn.Service.IService;
using LinkedLearn.Models.UserVM;
using Newtonsoft.Json.Linq;

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
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (content, statusCode) = await _gatewayService.RegisterAsync(model);

            if (statusCode == 200 || statusCode == 201)
            {
                TempData["SuccessMessage"] = "Đăng ký thành công! Mời bạn đăng nhập.";
                return RedirectToAction("Login");
            }

            string errorMessage = "Đã xảy ra lỗi.";
            try
            {
                var errorData = JObject.Parse(content);
                errorMessage = errorData["message"]?.ToString() ?? content;
            }
            catch { errorMessage = content; }

            ModelState.AddModelError(string.Empty, errorMessage);
            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (content, statusCode) = await _gatewayService.LoginAsync(model);

            if (statusCode == 200)
            {
                string token = "";
                try
                {
                    var data = JObject.Parse(content);
                    // Lấy giá trị chuỗi, tránh lấy cả Object JSON
                    token = data["token"]?.Value<string>() ?? data["accessToken"]?.Value<string>() ?? "";
                }
                catch
                {
                    token = content;
                }

                // BIỆN PHÁP MẠNH: Chỉ giữ lại ký tự ASCII hợp lệ (0-127)
                // Loại bỏ xuống dòng, khoảng trắng và dấu ngoặc kép thừa
                token = new string(token.Where(c => c <= 127).ToArray())
                    .Trim()
                    .Replace("\n", "")
                    .Replace("\r", "")
                    .Replace("\"", "");

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddDays(7),
                    Secure = false, // Localhost nên để false
                    SameSite = SameSiteMode.Lax
                };

                // Lưu Token sạch vào Cookie
                Response.Cookies.Append("jwt_token", token, cookieOptions);

                // Lưu UserName (Identifier thường là ASCII, nếu có dấu thì nên dùng WebUtility.UrlEncode)
                Response.Cookies.Append("UserName", model.Identifier, new CookieOptions { Expires = DateTime.Now.AddDays(7) });

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Tài khoản hoặc mật khẩu không chính xác.";

            // 2. Thêm vào ModelState để hỗ trợ các tag helper validation nếu có
            ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không chính xác.");

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("jwt_token");
            Response.Cookies.Delete("UserName");
            return RedirectToAction("Login");
        }
    }
}