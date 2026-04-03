using LinkedLearn.Models.UserVM;
using LinkedLearn.Service.IService;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http; // Cần thiết cho IHttpContextAccessor
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LinkedLearn.Service
{
    public class UserGatewayFrontendService : IUserGatewayFrontendService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string GatewayBaseUrl = "http://gateway-api:8080/api/User"; // Đổi thành localhost nếu chạy không Docker

        public UserGatewayFrontendService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Tạo HttpClient và đính kèm JWT Token từ Cookie vào Request Header
        /// </summary>
        private HttpClient CreateClientWithAuth()
        {
            var client = _httpClientFactory.CreateClient();
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["jwt_token"];

            if (!string.IsNullOrEmpty(token))
            {
                // Loại bỏ mọi ký tự không phải ASCII để an toàn tuyệt đối
                var cleanToken = new string(token.Where(c => c <= 127).ToArray()).Trim();

                // Gắn vào Header theo chuẩn Bearer
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", cleanToken);
            }
            return client;
        }

        public async Task<(string content, int statusCode)> RegisterAsync(RegisterViewModel model)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync($"{GatewayBaseUrl}/register", model);
            var content = await response.Content.ReadAsStringAsync();
            return (content, (int)response.StatusCode);
        }

        public async Task<(string content, int statusCode)> LoginAsync(LoginViewModel model)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var loginRequest = new
                {
                    identifier = model.Identifier,
                    password = model.Password
                };
                var response = await client.PostAsJsonAsync($"{GatewayBaseUrl}/login", loginRequest);
                var content = await response.Content.ReadAsStringAsync();
                return (content, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                return (ex.Message, 500);
            }
        }

        public async Task<(string content, int statusCode)> LogoutAsync()
        {
            var client = CreateClientWithAuth();
            var response = await client.PostAsync($"{GatewayBaseUrl}/logout", null);
            var content = await response.Content.ReadAsStringAsync();
            return (content, (int)response.StatusCode);
        }

        public async Task<(string content, int statusCode)> GetProfileAsync()
        {
            var client = CreateClientWithAuth();
            var response = await client.GetAsync($"{GatewayBaseUrl}/profile");
            var content = await response.Content.ReadAsStringAsync();
            return (content, (int)response.StatusCode);
        }

        public async Task<(string content, int statusCode)> UpdateProfileAsync(UpdateProfileViewModel model)
        {
            var client = CreateClientWithAuth();
            var response = await client.PutAsJsonAsync($"{GatewayBaseUrl}/update-profile", model);
            var content = await response.Content.ReadAsStringAsync();
            return (content, (int)response.StatusCode);
        }

        public async Task<(string content, int statusCode)> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            var client = CreateClientWithAuth();
            var response = await client.PostAsJsonAsync($"{GatewayBaseUrl}/change-password", model);
            var content = await response.Content.ReadAsStringAsync();
            return (content, (int)response.StatusCode);
        }

        public async Task<(string content, int statusCode)> UpdatePrivacyAsync(UpdatePrivacyViewModel model)
        {
            var client = CreateClientWithAuth();
            var response = await client.PutAsJsonAsync($"{GatewayBaseUrl}/privacy", model);
            var content = await response.Content.ReadAsStringAsync();
            return (content, (int)response.StatusCode);
        }
    }
}