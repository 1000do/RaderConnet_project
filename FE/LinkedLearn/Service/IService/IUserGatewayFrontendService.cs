using LinkedLearn.Models;
using LinkedLearn.Models.UserVM;

namespace LinkedLearn.Service.IService
{
    public interface IUserGatewayFrontendService
    {
        // ... các method cũ ...
        Task<(string token, int statusCode)> LoginAsync(LoginViewModel model);
        Task<(string content, int statusCode)> RegisterAsync(RegisterViewModel model);
    }
}
