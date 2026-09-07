using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RadarAPI.Models.DTO;
using RadarAPI.Services;
using System.Security.Claims;

namespace RadarAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Yêu cầu đăng nhập (có Token) cho mọi API ở đây
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        // 1. TẠO KÈO
        [HttpPost]
        public async Task<IActionResult> CreateActivity([FromBody] CreateActivityRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst("username")?.Value ?? User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

            if (userId == null) return Unauthorized();

            var result = await _activityService.CreateActivityAsync(request, userId, userName);
            return Ok(result);
        }

        // 2. TÌM KIẾM THEO BÁN KÍNH (RADAR)
        [HttpGet("radar")]
        public async Task<IActionResult> SearchActivities([FromQuery] double longitude, [FromQuery] double latitude, [FromQuery] double radius = 5000)
        {
            // Trả về danh sách Kèo trong bán kính (mặc định 5km = 5000m)
            var results = await _activityService.SearchActivitiesAsync(longitude, latitude, radius);
            return Ok(results);
        }

        // 3. XIN THAM GIA KÈO
        [HttpPost("{id}/join")]
        public async Task<IActionResult> JoinActivity(Guid id)
        {
            var userId = User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst("username")?.Value ?? User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

            if (userId == null) return Unauthorized();

            var message = await _activityService.JoinActivityAsync(id, userId, userName);
            return Ok(new { Message = message });
        }

        // 4. DUYỆT THÀNH VIÊN
        [HttpPut("{id}/participants/{participantId}/approve")]
        public async Task<IActionResult> ApproveParticipant(Guid id, string participantId)
        {
            var hostId = User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (hostId == null) return Unauthorized();

            var message = await _activityService.ApproveParticipantAsync(id, participantId, hostId);
            return Ok(new { Message = message });
        }

        // 5. TỪ CHỐI THÀNH VIÊN
        [HttpPut("{id}/participants/{participantId}/reject")]
        public async Task<IActionResult> RejectParticipant(Guid id, string participantId)
        {
            var hostId = User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (hostId == null) return Unauthorized();

            var message = await _activityService.RejectParticipantAsync(id, participantId, hostId);
            return Ok(new { Message = message });
        }
    }
}
