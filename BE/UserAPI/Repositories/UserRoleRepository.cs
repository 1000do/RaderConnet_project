using Microsoft.EntityFrameworkCore;
using UserAPI.Data;
using UserAPI.Models.Entities;
using UserAPI.Repositories.IRepositories;

namespace UserAPI.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly AppDbContext _context;

        public UserRoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserRole userRole)
        {
            await _context.UserRoles.AddAsync(userRole);
        }
    }
}