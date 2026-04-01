using UserAPI.Models.Entities;

namespace UserAPI.Repositories.IRepositories
{
    public interface IUserRoleRepository
    {
        Task AddAsync(UserRole userRole);
    }
}