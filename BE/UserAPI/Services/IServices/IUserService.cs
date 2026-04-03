using UserAPI.Models.DTO;
using UserAPI.Models.Entities;

namespace UserAPI.Services.IServices
{
    public interface IUserService
    {
        Task<string> Register(string username, string email, string password);
        Task<string> Login(string identifier, string password);
        Task<bool> ChangePassword(int userId, string oldPassword, string newPassword);
        Task<Profile?> GetMyProfile(int userId);
        Task<bool> UpdateProfile(int userId, UpdateProfileRequest request);
        Task<bool> UpdatePrivacy(int userId, UpdatePrivacyRequest request);
    }
}