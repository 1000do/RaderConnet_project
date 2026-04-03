using GateWay.Models;
using GateWay.Models.Auth;
using GateWay.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace GateWay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        // ================= AUTH =================

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
        {
            var (content, statusCode) = await _service.RegisterAsync(dto);
            // Trả về đúng StatusCode (400, 409...) và nội dung lỗi từ UserAPI
            return StatusCode(statusCode, content);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            var (content, statusCode) = await _service.LoginAsync(dto);
            return StatusCode(statusCode, content);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt_token");
            return Ok(new { message = "Logged out successfully" });
        }

        // ================= PROFILE (Cần Đăng nhập) =================

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var (content, statusCode) = await _service.GetProfileAsync();
            return StatusCode(statusCode, content);
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest dto)
        {
            var (content, statusCode) = await _service.UpdateProfileAsync(dto);
            return StatusCode(statusCode, content);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest dto)
        {
            var (content, statusCode) = await _service.ChangePasswordAsync(dto);
            return StatusCode(statusCode, content);
        }

        [HttpPut("privacy")]
        public async Task<IActionResult> UpdatePrivacy([FromBody] UpdatePrivacyRequest dto)
        {
            var (content, statusCode) = await _service.UpdatePrivacyAsync(dto);
            return StatusCode(statusCode, content);
        }
    }
}