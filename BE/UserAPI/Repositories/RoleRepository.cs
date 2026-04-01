using Microsoft.EntityFrameworkCore;
using UserAPI.Data;
using UserAPI.Models.Entities;
using UserAPI.Repositories.IRepositories;

namespace UserAPI.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Role> GetByNameAsync(string roleName)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == roleName);
        }
    }
}
