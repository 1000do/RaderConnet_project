namespace UserAPI.Models.DTO
{
    public class ChangePasswordRequest
    {
        // Bạn có thể lấy UserId từ Token, nên không cần truyền UserId ở đây để bảo mật
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}