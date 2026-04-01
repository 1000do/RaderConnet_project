using LinkedLearn.Models;

namespace LinkedLearn.Service.IService
{
    public interface IUserGatewayFrontendService
    {
        // ... các method cũ ...
        Task<(string token, int statusCode)> LoginAsync(LoginViewModel model);
        Task<(string content, int statusCode)> RegisterAsync(RegisterViewModel model);
    }
}
