using UserAPI.Models.Entities;

namespace UserAPI.Repositories.IRepositories
{
    public interface IRoleRepository
    {
        Task<Role> GetByNameAsync(string roleName);
    }
}