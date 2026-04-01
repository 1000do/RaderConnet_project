using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request) // Thêm [FromBody]
        {
            var result = await _service.Register(request.Username, request.Email, request.Password);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request) // Thêm [FromBody]
        {
            // Giờ đây request.Email và request.Password sẽ có giá trị từ JSON
            var token = await _service.Login(request.Email, request.Password);
            return Ok(new { Token = token });
        }

      
    }
}