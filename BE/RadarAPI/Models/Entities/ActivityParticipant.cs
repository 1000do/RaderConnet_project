using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadarAPI.Models.Entities
{
    [Table("ActivityParticipants")]
    public class ActivityParticipant
    {
        [Required]
        public Guid ActivityId { get; set; }

        [ForeignKey("ActivityId")]
        public Activity? Activity { get; set; }

        [Required]
        [MaxLength(255)]
        public required string UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public required string UserName { get; set; }

        public string? UserAvatarUrl { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public DateTime? JoinedAt { get; set; }
    }
}
