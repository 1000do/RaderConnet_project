using GateWay.Models;
using GateWay.Service.IService;
using Microsoft.AspNetCore.Http;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
        {
            var (content, statusCode) = await _service.RegisterAsync(dto);
            return StatusCode(statusCode, content);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            var (content, statusCode) = await _service.LoginAsync(dto);
            return StatusCode(statusCode, content);


        }
    }
}