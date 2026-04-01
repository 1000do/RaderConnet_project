namespace UserAPI.Services.IServices
{
    public interface IUserService
    {
        Task<string> Register(string username, string email, string password);
        Task<string> Login(string email, string password);
    }
}
    