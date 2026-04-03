namespace UserAPI.Models.DTO
{
    public class UpdateProfileRequest
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; } // Thêm trường này
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
    }
}