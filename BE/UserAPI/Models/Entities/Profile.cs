using UserAPI.Models.Entities;

public class Profile
{

    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string? FullName { get; set; }     // Dấu ? cho phép null
    public string? PhoneNumber { get; set; }  // Dấu ? cho phép null
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public string? Gender { get; set; }

    // 🔥 Các trường cấu hình riêng tư
    public bool IsPublicEmail { get; set; }
    public bool IsPublicPhone { get; set; }
    public bool IsPublicProfile { get; set; }

    public User User { get; set; }
}