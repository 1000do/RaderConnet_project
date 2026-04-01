using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UserAPI.Models.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; } // 🔥 thêm dòng này

        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }

        public Profile Profile { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}