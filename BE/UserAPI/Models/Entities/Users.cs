using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UserAPI.Models.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int Status { get; set; } = 1; // Khớp với DB (1: Active)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Profile Profile { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}