using LinkedLearn.Models.RadarVM;
using LinkedLearn.Service.IService;
using System.Net.Http.Json;
using System.Text.Json;

namespace LinkedLearn.Service
{
    public class RadarGatewayFrontendService : IRadarGatewayFrontendService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        // Đi qua Ocelot Gateway với đường dẫn /api/Radar/Activity
        private const string GatewayBaseUrl = "http://gateway-api:8080/api/Radar/Activity";

        public RadarGatewayFrontendService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient CreateClientWithAuth()
        {
            var client = _httpClientFactory.CreateClient();
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["jwt_token"];

            if (!string.IsNullOrEmpty(token))
            {
                var cleanToken = new string(token.Where(c => c <= 127).ToArray()).Trim();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", cleanToken);
            }
            return client;
        }

        public async Task<(string content, int statusCode)> CreateActivityAsync(CreateActivityViewModel model)
        {
            var client = CreateClientWithAuth();
            var response = await client.PostAsJsonAsync(GatewayBaseUrl, model);
            var content = await response.Content.ReadAsStringAsync();
            return (content, (int)response.StatusCode);
        }

        public async Task<IEnumerable<ActivityViewModel>> SearchRadarAsync(double longitude, double latitude, double radiusInMeters = 5000)
        {
            var client = CreateClientWithAuth();
            var response = await client.GetAsync($"{GatewayBaseUrl}/radar?longitude={longitude}&latitude={latitude}&radius={radiusInMeters}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<IEnumerable<ActivityViewModel>>(content, options) ?? new List<ActivityViewModel>();
            }
            return new List<ActivityViewModel>();
        }

        public async Task<(string content, int statusCode)> JoinActivityAsync(Guid activityId)
        {
            var client = CreateClientWithAuth();
            var response = await client.PostAsync($"{GatewayBaseUrl}/{activityId}/join", null);
            var content = await response.Content.ReadAsStringAsync();
            return (content, (int)response.StatusCode);
        }

        public async Task<(string content, int statusCode)> ApproveParticipantAsync(Guid activityId, string participantId)
        {
            var client = CreateClientWithAuth();
            var response = await client.PutAsync($"{GatewayBaseUrl}/{activityId}/participants/{participantId}/approve", null);
            var content = await response.Content.ReadAsStringAsync();
            return (content, (int)response.StatusCode);
        }

        public async Task<(string content, int statusCode)> RejectParticipantAsync(Guid activityId, string participantId)
        {
            var client = CreateClientWithAuth();
            var response = await client.PutAsync($"{GatewayBaseUrl}/{activityId}/participants/{participantId}/reject", null);
            var content = await response.Content.ReadAsStringAsync();
            return (content, (int)response.StatusCode);
        }
    }
}
