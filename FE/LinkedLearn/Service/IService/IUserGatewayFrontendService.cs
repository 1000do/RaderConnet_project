using LinkedLearn.Models.UserVM;

namespace LinkedLearn.Service.IService
{
    public interface IUserGatewayFrontendService
    {
        Task<(string content, int statusCode)> RegisterAsync(RegisterViewModel model);
        Task<(string content, int statusCode)> LoginAsync(LoginViewModel model);
        Task<(string content, int statusCode)> LogoutAsync();
        Task<(string content, int statusCode)> GetProfileAsync();
        Task<(string content, int statusCode)> UpdateProfileAsync(UpdateProfileViewModel model);
        Task<(string content, int statusCode)> ChangePasswordAsync(ChangePasswordViewModel model);
        Task<(string content, int statusCode)> UpdatePrivacyAsync(UpdatePrivacyViewModel model);
    }
}