using GateWay.Models;
using GateWay.Service.IService;
using System.Net.Http.Json;

namespace GateWay.Service
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;
        private readonly IHostEnvironment _env;

        public UserService(
            IConfiguration configuration,
            ILogger<UserService> logger,
            IHostEnvironment env)
        {
            _configuration = configuration;
            _logger = logger;
            _env = env;
        }

        private HttpClient CreateClient()
        {
            return new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (msg, cert, chain, errors) => !_env.IsProduction()
            });
        }

        private string GetUserUrl(string endpoint)
        {
            var baseUrl = _configuration["GatewaySettings:UserServiceUrl"];

            // Nếu config bị lỗi không đọc được, hãy gán mặc định để test
            if (string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = "http://user-api:8080";
            }

            return $"{baseUrl.TrimEnd('/')}/api/{endpoint}";
        }

        // ================= REGISTER =================
        public async Task<(string content, int statusCode)> RegisterAsync(RegisterRequest dto)
        {
            try
            {
                var client = CreateClient();
                var url = GetUserUrl("Auth/register");

                _logger.LogInformation($"Calling Register API: {url}");

                var response = await client.PostAsJsonAsync(url, dto);
                var content = await response.Content.ReadAsStringAsync();

                return (content, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Register");
                return (ex.Message, 500);
            }
        }

        // ================= LOGIN =================
        public async Task<(string content, int statusCode)> LoginAsync(LoginRequest dto)
        {
            var client = CreateClient();
            var url = GetUserUrl("Auth/login");

            var response = await client.PostAsJsonAsync(url, dto);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Login failed: {content}");
                // Bạn có thể tùy chỉnh content ở đây nếu muốn giấu bớt thông tin hệ thống
            }

            return (content, (int)response.StatusCode);
        }

        // ================= GET USERS =================
        public async Task<(string content, int statusCode)> GetUsersAsync()
        {
            try
            {
                var client = CreateClient();
                var url = GetUserUrl("User");

                _logger.LogInformation($"Calling GetUsers API: {url}");

                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                return (content, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetUsers");
                return (ex.Message, 500);
            }
        }
    }
}