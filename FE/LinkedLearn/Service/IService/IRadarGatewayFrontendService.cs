using LinkedLearn.Models.RadarVM;

namespace LinkedLearn.Service.IService
{
    public interface IRadarGatewayFrontendService
    {
        Task<(string content, int statusCode)> CreateActivityAsync(CreateActivityViewModel model);
        Task<IEnumerable<ActivityViewModel>> SearchRadarAsync(double longitude, double latitude, double radiusInMeters = 5000);
        Task<(string content, int statusCode)> JoinActivityAsync(Guid activityId);
        Task<(string content, int statusCode)> ApproveParticipantAsync(Guid activityId, string participantId);
        Task<(string content, int statusCode)> RejectParticipantAsync(Guid activityId, string participantId);
    }
}
