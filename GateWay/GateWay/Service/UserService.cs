using GateWay.Models;
using GateWay.Service.IService;
using System.Net.Http.Json;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using GateWay.Models.Auth;
using System.Net.Http.Headers;

namespace GateWay.Service
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;
        private readonly IHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            IConfiguration configuration,
            ILogger<UserService> logger,
            IHostEnvironment env,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _logger = logger;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient CreateClient()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => !_env.IsProduction()
            };

            var client = new HttpClient(handler);

            // 1. Lấy Token từ Cookie (mà Gateway đã lưu khi Login)
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["jwt_token"];

            // 2. Nếu có Token, gắn vào Header Authorization để Backend (UserAPI) xác thực được
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // 3. (Optional) Gắn thêm X-User-Id nếu Gateway đã verify được User
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirst("userId")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    client.DefaultRequestHeaders.Add("X-User-Id", userId);
                }
            }

            return client;
        }

        private string GetUserUrl(string endpoint) =>
            $"{(_configuration["GatewaySettings:UserServiceUrl"] ?? "http://localhost:8080").TrimEnd('/')}/api/{endpoint}";

        // ================= AUTH LOGIC =================

        public async Task<(string content, int statusCode)> RegisterAsync(RegisterRequest dto)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync(GetUserUrl("auth/register"), dto);
            return (await response.Content.ReadAsStringAsync(), (int)response.StatusCode);
        }

        public async Task<(string content, int statusCode)> LoginAsync(LoginRequest dto)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync(GetUserUrl("auth/login"), dto);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var json = JObject.Parse(content);
                    var token = json["token"]?.ToString() ?? json["Token"]?.ToString();

                    if (!string.IsNullOrEmpty(token))
                    {
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = false, // Để false khi test ở localhost (no HTTPS)
                            SameSite = SameSiteMode.Lax, // Đổi từ Strict sang Lax để dễ test với Swagger
                            Expires = DateTime.UtcNow.AddHours(2)
                        };

                        _httpContextAccessor.HttpContext?.Response.Cookies.Append("jwt_token", token, cookieOptions);
                        return ("{\"message\": \"Login thành công, Cookie đã được thiết lập.\"}", 200);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi parse Token từ UserAPI");
                }
            }
            return (content, (int)response.StatusCode);
        }

        // ================= PROFILE LOGIC =================

        public async Task<(string content, int statusCode)> GetProfileAsync()
        {
            var client = CreateClient();
            var response = await client.GetAsync(GetUserUrl("auth/get-profile"));
            var content = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(content) && response.IsSuccessStatusCode)
            {
                content = "{\"info\": \"Backend trả về 200 nhưng body rỗng. Kiểm tra lại User-API.\"}";
            }

            return (content, (int)response.StatusCode);
        }

        public async Task<(string content, int statusCode)> UpdateProfileAsync(UpdateProfileRequest dto)
        {
            var client = CreateClient();
            var response = await client.PutAsJsonAsync(GetUserUrl("auth/update-profile"), dto);
            return (await response.Content.ReadAsStringAsync(), (int)response.StatusCode);
        }

        public async Task<(string content, int statusCode)> ChangePasswordAsync(ChangePasswordRequest dto)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync(GetUserUrl("auth/change-password"), dto);
            return (await response.Content.ReadAsStringAsync(), (int)response.StatusCode);
        }

        public async Task<(string content, int statusCode)> UpdatePrivacyAsync(UpdatePrivacyRequest dto)
        {
            var client = CreateClient();
            var response = await client.PutAsJsonAsync(GetUserUrl("auth/privacy"), dto);
            return (await response.Content.ReadAsStringAsync(), (int)response.StatusCode);
        }

        public async Task<(string content, int statusCode)> GetUsersAsync()
        {
            var client = CreateClient();
            var response = await client.GetAsync(GetUserUrl("User"));
            return (await response.Content.ReadAsStringAsync(), (int)response.StatusCode);
        }
    }
}