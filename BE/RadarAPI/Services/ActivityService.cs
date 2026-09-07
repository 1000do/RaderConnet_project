using NetTopologySuite.Geometries;
using RadarAPI.Models.DTO;
using RadarAPI.Models.Entities;
using RadarAPI.Repositories;

namespace RadarAPI.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;

        public ActivityService(IActivityRepository activityRepository)
        {
            _activityRepository = activityRepository;
        }

        public async Task<ActivityResponse> CreateActivityAsync(CreateActivityRequest request, string hostId, string hostName)
        {
            var activity = new Activity
            {
                Title = request.Title,
                Description = request.Description,
                CategoryId = request.CategoryId,
                HostId = hostId,
                HostName = hostName,
                // NetTopologySuite quy định X là Longitude, Y là Latitude. Đừng nhầm lẫn!
                Location = new Point(request.Longitude, request.Latitude) { SRID = 4326 },
                MaxParticipants = request.MaxParticipants,
                ExpiresAt = request.ExpiresAt.ToUniversalTime()
            };

            await _activityRepository.CreateActivityAsync(activity);

            return MapToResponse(activity, null);
        }

        public async Task<IEnumerable<ActivityResponse>> SearchActivitiesAsync(double longitude, double latitude, double radiusInMeters)
        {
            var activities = await _activityRepository.SearchActivitiesInRadiusAsync(longitude, latitude, radiusInMeters);
            var userLocation = new Point(longitude, latitude) { SRID = 4326 };

            return activities.Select(a => MapToResponse(a, userLocation));
        }

        public async Task<string> JoinActivityAsync(Guid activityId, string userId, string userName)
        {
            var activity = await _activityRepository.GetActivityByIdAsync(activityId);
            if (activity == null || activity.Status != "Open")
            {
                return "Kèo không tồn tại hoặc đã đóng/hết hạn.";
            }

            if (activity.HostId == userId)
            {
                return "Bạn là chủ kèo, không thể xin tham gia kèo của chính mình.";
            }

            var existingParticipant = await _activityRepository.GetParticipantAsync(activityId, userId);
            if (existingParticipant != null)
            {
                return "Bạn đã gửi yêu cầu tham gia kèo này rồi.";
            }

            var participant = new ActivityParticipant
            {
                ActivityId = activityId,
                UserId = userId,
                UserName = userName,
                Status = "Pending"
            };

            await _activityRepository.AddParticipantAsync(participant);
            return "Xin tham gia thành công. Vui lòng chờ Host duyệt.";
        }

        public async Task<string> ApproveParticipantAsync(Guid activityId, string participantId, string hostId)
        {
            var activity = await _activityRepository.GetActivityByIdAsync(activityId);
            if (activity == null) return "Kèo không tồn tại.";
            if (activity.HostId != hostId) return "Chỉ chủ kèo mới có quyền duyệt.";

            var participant = await _activityRepository.GetParticipantAsync(activityId, participantId);
            if (participant == null || participant.Status != "Pending") return "Yêu cầu không hợp lệ.";

            if (activity.CurrentParticipants >= activity.MaxParticipants)
            {
                activity.Status = "Full";
                await _activityRepository.UpdateActivityAsync(activity);
                return "Kèo đã đủ người, không thể duyệt thêm.";
            }

            // TODO: Ở giai đoạn sau sẽ thêm Optimistic Concurrency vào đây để khóa tranh chấp
            participant.Status = "Approved";
            participant.JoinedAt = DateTime.UtcNow;
            await _activityRepository.UpdateParticipantAsync(participant);

            activity.CurrentParticipants += 1;
            if (activity.CurrentParticipants >= activity.MaxParticipants)
            {
                activity.Status = "Full";
            }
            await _activityRepository.UpdateActivityAsync(activity);

            return "Đã duyệt thành công.";
        }

        public async Task<string> RejectParticipantAsync(Guid activityId, string participantId, string hostId)
        {
            var activity = await _activityRepository.GetActivityByIdAsync(activityId);
            if (activity == null || activity.HostId != hostId) return "Không có quyền.";

            var participant = await _activityRepository.GetParticipantAsync(activityId, participantId);
            if (participant == null || participant.Status != "Pending") return "Yêu cầu không hợp lệ.";

            participant.Status = "Rejected";
            await _activityRepository.UpdateParticipantAsync(participant);

            return "Đã từ chối thành công.";
        }

        private ActivityResponse MapToResponse(Activity activity, Point? userLocation)
        {
            return new ActivityResponse
            {
                Id = activity.Id,
                HostId = activity.HostId,
                HostName = activity.HostName,
                HostAvatarUrl = activity.HostAvatarUrl,
                Title = activity.Title,
                Description = activity.Description,
                CategoryId = activity.CategoryId,
                Longitude = activity.Location.X,
                Latitude = activity.Location.Y,
                // Tính khoảng cách (mét) nếu có tọa độ của user truyền vào
                DistanceInMeters = userLocation != null ? activity.Location.Distance(userLocation) : null,
                MaxParticipants = activity.MaxParticipants,
                CurrentParticipants = activity.CurrentParticipants,
                Status = activity.Status,
                CreatedAt = activity.CreatedAt,
                ExpiresAt = activity.ExpiresAt
            };
        }
    }
}
