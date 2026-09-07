using RadarAPI.Models.DTO;

namespace RadarAPI.Services
{
    public interface IActivityService
    {
        Task<ActivityResponse> CreateActivityAsync(CreateActivityRequest request, string hostId, string hostName);
        
        Task<IEnumerable<ActivityResponse>> SearchActivitiesAsync(double longitude, double latitude, double radiusInMeters);
        
        Task<string> JoinActivityAsync(Guid activityId, string userId, string userName);
        
        Task<string> ApproveParticipantAsync(Guid activityId, string participantId, string hostId);
        
        Task<string> RejectParticipantAsync(Guid activityId, string participantId, string hostId);
    }
}
