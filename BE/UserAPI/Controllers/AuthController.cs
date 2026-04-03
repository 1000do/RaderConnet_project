using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using UserAPI.Models.DTO;
using UserAPI.Services.IServices;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _service;

        public AuthController(IUserService service)
        {
            _service = service;
        }

        // --- ĐĂNG KÝ ---
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _service.Register(request.Username, request.Email, request.Password);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                // Trả về lỗi 400 kèm nội dung tin nhắn thay vì ném Exception làm sập luồng
                return BadRequest(new { message = ex.Message });
            }
        }

        // --- ĐĂNG NHẬP ---
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _service.Login(request.Identifier, request.Password);
            return Ok(new { Token = token });
        }

        // --- LẤY THÔNG TIN CÁ NHÂN (ĐỔI TÊN THÀNH GET-PROFILE) ---
        [Authorize]
        [HttpGet("get-profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized();

            var profile = await _service.GetMyProfile(userId.Value);
            if (profile == null) return NotFound("Không tìm thấy hồ sơ.");

            // Trả về cả những trường có thể đang NULL để Frontend hiển thị ô trống
            return Ok(new
            {
                Email = profile.User?.Email,
                profile.Username,
                FullName = profile.FullName ?? "", // Nếu null thì trả về chuỗi rỗng
                PhoneNumber = profile.PhoneNumber ?? "",
                Bio = profile.Bio ?? "",
                AvatarUrl = profile.AvatarUrl ?? "",
                profile.IsInstructor,
                PrivacySettings = new
                {
                    profile.IsPublicEmail,
                    profile.IsPublicPhone,
                    profile.IsPublicProfile
                }
            });
        }

        // --- CẬP NHẬT/THÊM MỚI THÔNG TIN (VỪA LÀ ADD VỪA LÀ EDIT) ---
        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized();

            // Hàm này sẽ ghi đè dữ liệu mới vào các trường cũ (kể cả đang trống)
            var result = await _service.UpdateProfile(userId.Value, request);

            if (result) return Ok(new { message = "Cập nhật thông tin thành công!" });
            return BadRequest("Không thể cập nhật thông tin.");
        }

        // --- UC-10: CẤU HÌNH QUYỀN RIÊNG TƯ ---
        [Authorize]
        [HttpPut("update-privacy")]
        public async Task<IActionResult> UpdatePrivacy([FromBody] UpdatePrivacyRequest request)
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized();

            var result = await _service.UpdatePrivacy(userId.Value, request);

            if (result) return Ok(new { message = "Cập nhật quyền riêng tư thành công!" });
            return BadRequest("Không thể cập nhật quyền riêng tư.");    
        }

        // --- ĐỔI MẬT KHẨU ---
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized();

            var result = await _service.ChangePassword(userId.Value, request.OldPassword, request.NewPassword);
            if (result) return Ok(new { message = "Mật khẩu đã được đổi và băm an toàn!" });
            return BadRequest("Mật khẩu cũ không đúng.");
        }

        private int? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return string.IsNullOrEmpty(userIdClaim) ? null : int.Parse(userIdClaim);
        }
    }
}