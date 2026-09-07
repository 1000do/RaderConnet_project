using RadarAPI.Models.Entities;

namespace RadarAPI.Repositories
{
    public interface IActivityRepository
    {
        Task<Activity> CreateActivityAsync(Activity activity);
        Task<Activity?> GetActivityByIdAsync(Guid activityId);
        
        // Hàm TRÁI TIM của PostGIS: Tìm các Kèo trong bán kính (mét)
        Task<IEnumerable<Activity>> SearchActivitiesInRadiusAsync(double longitude, double latitude, double radiusInMeters);
        
        Task<ActivityParticipant> AddParticipantAsync(ActivityParticipant participant);
        Task<ActivityParticipant?> GetParticipantAsync(Guid activityId, string userId);
        Task UpdateParticipantAsync(ActivityParticipant participant);
        Task UpdateActivityAsync(Activity activity);
    }
}
