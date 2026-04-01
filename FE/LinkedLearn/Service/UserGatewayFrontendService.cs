
using LinkedLearn.Models;
using LinkedLearn.Service.IService;

namespace LinkedLearn.Service
{
    public class UserGatewayFrontendService : IUserGatewayFrontendService
{
    private readonly IHttpClientFactory _httpClientFactory;
      private const string GatewayBaseUrl = "http://gateway-api:8080/api/User";
        public UserGatewayFrontendService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<(string token, int statusCode)> LoginAsync(LoginViewModel model)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync($"{GatewayBaseUrl}/login", model);
            var content = await response.Content.ReadAsStringAsync();

            return (content, (int)response.StatusCode);
        }

        public async Task<(string content, int statusCode)> RegisterAsync(RegisterViewModel model)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync($"{GatewayBaseUrl}/register", model);
            var content = await response.Content.ReadAsStringAsync();

            return (content, (int)response.StatusCode);
        }
    }
}
