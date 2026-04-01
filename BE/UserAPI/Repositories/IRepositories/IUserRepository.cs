using UserAPI.Models.Entities;

namespace UserAPI.Repositories.IRepositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUsernameAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);

        Task AddAsync(User user);
        Task SaveChangesAsync();
    }
}