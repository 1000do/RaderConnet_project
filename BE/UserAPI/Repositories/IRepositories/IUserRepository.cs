using UserAPI.Models.Entities;

namespace UserAPI.Repositories.IRepositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByLoginIdentifierAsync(string identifier);
        Task<bool> EmailExistsAsync(string email);
        Task AddAsync(User user);
        Task SaveChangesAsync();
    }
}