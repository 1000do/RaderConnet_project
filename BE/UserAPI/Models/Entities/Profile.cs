using UserAPI.Models.Entities;

public class Profile
{
    public int UserId { get; set; } // PK + FK
    public string FullName { get; set; }
    public string AvatarUrl { get; set; }
    public string Bio { get; set; }
    public bool IsInstructor { get; set; }

    public User User { get; set; }
}