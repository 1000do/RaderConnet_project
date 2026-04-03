using GateWay.Models.Auth;

namespace GateWay.Service.IService
{
    public interface IUserService
    {
        // Auth & Profile
        Task<(string content, int statusCode)> RegisterAsync(RegisterRequest dto);
        Task<(string content, int statusCode)> LoginAsync(LoginRequest dto);
        Task<(string content, int statusCode)> GetProfileAsync();
        Task<(string content, int statusCode)> UpdateProfileAsync(UpdateProfileRequest dto);
        Task<(string content, int statusCode)> ChangePasswordAsync(ChangePasswordRequest dto);
        Task<(string content, int statusCode)> UpdatePrivacyAsync(UpdatePrivacyRequest dto);

        // Admin/General
        Task<(string content, int statusCode)> GetUsersAsync();
    }
}