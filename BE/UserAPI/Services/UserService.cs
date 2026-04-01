using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserAPI.Models.Entities;
using UserAPI.Repositories.IRepositories;
using UserAPI.Services.IServices;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IUserRoleRepository _userRoleRepo;
    private readonly IRoleRepository _roleRepo;
    private readonly IConfiguration _config;

    public UserService(
        IUserRepository userRepo,
        IUserRoleRepository userRoleRepo,
        IRoleRepository roleRepo,
        IConfiguration config)
    {
        _userRepo = userRepo;
        _userRoleRepo = userRoleRepo;
        _roleRepo = roleRepo;
        _config = config;
    }

    public async Task<string> Register(string username, string email, string password)
    {
        if (await _userRepo.EmailExistsAsync(email))
            throw new Exception("Email exists");

        if (await _userRepo.UsernameExistsAsync(username))
            throw new Exception("Username exists");

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();

        // 🔥 Lấy role learner từ DB
        var role = await _roleRepo.GetByNameAsync("learner");

        var userRole = new UserRole
        {
            UserId = user.UserId,
            RoleId = role.RoleId
        };

        await _userRoleRepo.AddAsync(userRole);
        await _userRepo.SaveChangesAsync();

        return "Register success";
    }


    public async Task<string> Login(string email, string password)
    {
        var user = await _userRepo.GetByEmailAsync(email);

        // Log độ dài để xem có khoảng trắng ẩn không
        Console.WriteLine($"Login Email: '{email}' - Length: {email.Length}");
        Console.WriteLine($"Input Password: '{password}' - Length: {password.Length}");

        if (user == null)
        {
            Console.WriteLine("LỖI: Không tìm thấy User với email này!");
            throw new Exception("Invalid credentials");
        }

        bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!isValid)
        {
            Console.WriteLine("LỖI: Mật khẩu không khớp với Hash trong DB!");
            throw new Exception("Invalid credentials");
        }

        return GenerateJwt(user);
    }

    private string GenerateJwt(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"])
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("userId", user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

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