using Microsoft.EntityFrameworkCore;
using UserAPI.Data;
using UserAPI.Models.Entities;
using UserAPI.Repositories.IRepositories;

namespace UserAPI.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly AppDbContext _context;

        public ProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Profile?> GetByUserIdAsync(int userId)
        {
            // Dùng Include nếu bạn muốn lấy luôn Email từ bảng User để hiển thị ở trang Profile
            return await _context.Profiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            // Kiểm tra username ở bảng Profiles theo thiết kế DB mới
            return await _context.Profiles.AnyAsync(p => p.Username == username);
        }

        public async Task UpdateAsync(Profile profile)
        {
            _context.Profiles.Update(profile);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}