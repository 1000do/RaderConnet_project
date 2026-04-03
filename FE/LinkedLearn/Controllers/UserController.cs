using LinkedLearn.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using LinkedLearn.Models.UserVM;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace LinkedLearn.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserGatewayFrontendService _gatewayService;

        public UserController(IUserGatewayFrontendService gatewayService)
        {
            _gatewayService = gatewayService;
        }

        // --- TRANG XEM THÔNG TIN CÁ NHÂN ---
        public async Task<IActionResult> Profile()
        {
            var (content, statusCode) = await _gatewayService.GetProfileAsync();

            if (statusCode == 200)
            {
                var userProfile = JsonConvert.DeserializeObject<UpdateProfileViewModel>(content);

                if (userProfile != null)
                {
                    // Cập nhật lại Cookie "UserName" để Navbar luôn hiển thị đúng
                    // Vì Model mới không còn .Username, ta dùng FullName hoặc fallback về Cookie cũ nếu cần
                    string displayName = !string.IsNullOrEmpty(userProfile.FullName)
                                         ? userProfile.FullName
                                         : Request.Cookies["UserName"] ?? "User";

                    Response.Cookies.Append("UserName", displayName, new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7),
                        HttpOnly = false
                    });

                    // Lưu Avatar nếu có
                    if (!string.IsNullOrEmpty(userProfile.AvatarUrl))
                    {
                        Response.Cookies.Append("UserAvatar", userProfile.AvatarUrl, new CookieOptions { Expires = DateTime.Now.AddDays(7) });
                    }
                }

                return View(userProfile);
            }

            return RedirectToAction("Login", "Account");
        }

        // --- TRANG CHỈNH SỬA THÔNG TIN ---
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var (content, statusCode) = await _gatewayService.GetProfileAsync();

            if (statusCode == 200)
            {
                var userProfile = JsonConvert.DeserializeObject<UpdateProfileViewModel>(content);
                return View(userProfile);
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (content, statusCode) = await _gatewayService.UpdateProfileAsync(model);

            if (statusCode == 200)
            {
                TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
                // Redirect về Profile để gán lại Cookie tên mới
                return RedirectToAction("Profile");
            }

            // Hiển thị thông báo lỗi từ API nếu có
            ViewBag.ErrorMessage = "Cập nhật thất bại: " + content;
            return View(model);
        }
    }
}