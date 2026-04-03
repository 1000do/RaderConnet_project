using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserAPI.Models.DTO;
using UserAPI.Models.Entities;
using UserAPI.Repositories.IRepositories;
using UserAPI.Services.IServices;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IProfileRepository _profileRepo;
    private readonly IUserRoleRepository _userRoleRepo;
    private readonly IRoleRepository _roleRepo;
    private readonly IConfiguration _config;

    public UserService(
        IUserRepository userRepo,
        IProfileRepository profileRepo,
        IUserRoleRepository userRoleRepo,
        IRoleRepository roleRepo,
        IConfiguration config)
    {
        _userRepo = userRepo;
        _profileRepo = profileRepo;
        _userRoleRepo = userRoleRepo;
        _roleRepo = roleRepo;
        _config = config;
    }

    public async Task<string> Register(string username, string email, string password)
    {
        // Kiểm tra Email (Chuyển về chữ thường để so sánh)
        if (await _userRepo.EmailExistsAsync(email.ToLower()))
            throw new Exception("Email này đã được sử dụng ");

        // Kiểm tra Username (Chuyển về chữ thường để so sánh)
        if (await _profileRepo.UsernameExistsAsync(username.ToLower()))
            throw new Exception("Tên đăng nhập đã tồn tại.");

        var user = new User
        {
            Email = email.ToLower(), // Luôn lưu email ở dạng chữ thường
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Status = 1,
            CreatedAt = DateTime.UtcNow,
            Profile = new Profile
            {
                Username = username,
                FullName = username,
                IsInstructor = false,
                IsPublicProfile = true
            }
        };

        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();

        var role = await _roleRepo.GetByNameAsync("learner");
        if (role != null)
        {
            await _userRoleRepo.AddAsync(new UserRole { UserId = user.UserId, RoleId = role.RoleId });
            await _userRepo.SaveChangesAsync();
        }

        return "Đăng ký thành công";
    }

    public async Task<string> Login(string identifier, string password)
    {
        // Tìm user (Repository của bạn phải hỗ trợ tìm cả Email/Username)
        var user = await _userRepo.GetByLoginIdentifierAsync(identifier.ToLower());

        if (user == null) throw new Exception("Tài khoản không tồn tại.");
        if (user.Status == 0) throw new Exception("Tài khoản của bạn đã bị khóa.");

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new Exception("Mật khẩu không chính xác.");

        return GenerateJwt(user);
    }

    // Các hàm ChangePassword, UpdateProfile... giữ nguyên nội dung cũ của bạn
    public async Task<bool> ChangePassword(int userId, string oldPassword, string newPassword)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null || !BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
            throw new Exception("Mật khẩu cũ không chính xác!");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _userRepo.SaveChangesAsync();
        return true;
    }

    public async Task<Profile?> GetMyProfile(int userId) => await _profileRepo.GetByUserIdAsync(userId);

    public async Task<bool> UpdateProfile(int userId, UpdateProfileRequest request)
    {
        var profile = await _profileRepo.GetByUserIdAsync(userId);
        if (profile == null) return false;
        profile.FullName = request.FullName;
        profile.PhoneNumber = request.PhoneNumber;
        profile.Bio = request.Bio;
        profile.AvatarUrl = request.AvatarUrl;
        await _profileRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatePrivacy(int userId, UpdatePrivacyRequest request)
    {
        var profile = await _profileRepo.GetByUserIdAsync(userId);
        if (profile == null) return false;
        profile.IsPublicEmail = request.IsPublicEmail;
        profile.IsPublicPhone = request.IsPublicPhone;
        profile.IsPublicProfile = request.IsPublicProfile;
        await _profileRepo.SaveChangesAsync();
        return true;
    }

    private string GenerateJwt(User user)
    {
        var jwtKey = _config["Jwt:Key"] ?? throw new Exception("Jwt:Key is missing");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim("userId", user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("username", user.Profile?.Username ?? "")
        };

        if (user.UserRoles != null)
        {
            foreach (var ur in user.UserRoles)
                claims.Add(new Claim(ClaimTypes.Role, ur.Role.RoleName));
        }

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}