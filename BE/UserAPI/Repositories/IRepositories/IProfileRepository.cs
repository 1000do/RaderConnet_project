using UserAPI.Models.Entities;

namespace UserAPI.Repositories.IRepositories
{
    public interface IProfileRepository
    {
        Task<Profile?> GetByUserIdAsync(int userId);
        Task UpdateAsync(Profile profile);
        Task SaveChangesAsync();
        Task<bool> UsernameExistsAsync(string username);
    }
}