using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadarAPI.Models.Entities
{
    [Table("Activities")]
    public class Activity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public required string HostId { get; set; }

        [Required]
        [MaxLength(255)]
        public required string HostName { get; set; }

        public string? HostAvatarUrl { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        // TRÁI TIM CỦA RADAR: Kiểu dữ liệu Point của NetTopologySuite
        // Dùng để lưu Tọa độ địa lý chuẩn WGS 84 (SRID 4326)
        [Required]
        [Column(TypeName = "geography(Point, 4326)")]
        public required Point Location { get; set; }

        [Required]
        public int MaxParticipants { get; set; }

        public int CurrentParticipants { get; set; } = 0;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Open"; // Open, Full, Expired, Cancelled

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime ExpiresAt { get; set; }

        public ICollection<ActivityParticipant> Participants { get; set; } = new List<ActivityParticipant>();
    }
}
