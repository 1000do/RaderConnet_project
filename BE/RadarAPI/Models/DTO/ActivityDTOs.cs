using System.ComponentModel.DataAnnotations;

namespace RadarAPI.Models.DTO
{
    public class CreateActivityRequest
    {
        [Required]
        [MaxLength(255)]
        public required string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double Longitude { get; set; }

        [Required]
        [Range(1, 100)]
        public int MaxParticipants { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }
    }

    public class ActivityResponse
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
        public double? DistanceInMeters { get; set; } // Dành cho lúc quét Radar

        public int MaxParticipants { get; set; }
        public int CurrentParticipants { get; set; }
        public string Status { get; set; } = null!;
        
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
