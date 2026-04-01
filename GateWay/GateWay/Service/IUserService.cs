using GateWay.Models;

namespace GateWay.Service
{
    public interface IUserService
    {
        Task<(string content, int statusCode)> RegisterAsync(RegisterRequest dto);
        Task<(string content, int statusCode)> LoginAsync(LoginRequest dto);
        Task<(string content, int statusCode)> GetUsersAsync();
    }
}
