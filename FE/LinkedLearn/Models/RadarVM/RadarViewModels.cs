using System.ComponentModel.DataAnnotations;

namespace LinkedLearn.Models.RadarVM
{
    public class CreateActivityViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề Kèo")]
        [MaxLength(255)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng cho phép lấy tọa độ GPS")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "Vui lòng cho phép lấy tọa độ GPS")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "Số người cần tuyển?")]
        [Range(1, 100)]
        public int MaxParticipants { get; set; }

        [Required(ErrorMessage = "Khi nào hết hạn?")]
        public DateTime ExpiresAt { get; set; }
    }

    public class ActivityViewModel
    {
        public Guid Id { get; set; }
        public string HostId { get; set; } = null!;
        public string HostName { get; set; } = null!;
        public string? HostAvatarUrl { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? DistanceInMeters { get; set; }

        public int MaxParticipants { get; set; }
        public int CurrentParticipants { get; set; }
        public string Status { get; set; } = null!;
        
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
