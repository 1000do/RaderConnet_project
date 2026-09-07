using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using RadarAPI.Data;
using RadarAPI.Models.Entities;

namespace RadarAPI.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly AppDbContext _context;

        public ActivityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Activity> CreateActivityAsync(Activity activity)
        {
            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();
            return activity;
        }

        public async Task<Activity?> GetActivityByIdAsync(Guid activityId)
        {
            return await _context.Activities
                .FirstOrDefaultAsync(a => a.Id == activityId);
        }

        public async Task<IEnumerable<Activity>> SearchActivitiesInRadiusAsync(double longitude, double latitude, double radiusInMeters)
        {
            // Tạo đối tượng Point từ tọa độ người dùng (Longitude trước, Latitude sau theo chuẩn x, y)
            var userLocation = new Point(longitude, latitude) { SRID = 4326 };

            // Tìm các Kèo:
            // 1. Còn trạng thái Open
            // 2. Nằm trong bán kính radiusInMeters so với userLocation
            // Sắp xếp theo khoảng cách gần nhất
            return await _context.Activities
                .Where(a => a.Status == "Open" && a.Location.Distance(userLocation) <= radiusInMeters)
                .OrderBy(a => a.Location.Distance(userLocation))
                .Take(50) // Giới hạn trả về 50 kèo gần nhất
                .ToListAsync();
        }

        public async Task<ActivityParticipant> AddParticipantAsync(ActivityParticipant participant)
        {
            _context.ActivityParticipants.Add(participant);
            await _context.SaveChangesAsync();
            return participant;
        }

        public async Task<ActivityParticipant?> GetParticipantAsync(Guid activityId, string userId)
        {
            return await _context.ActivityParticipants
                .FirstOrDefaultAsync(p => p.ActivityId == activityId && p.UserId == userId);
        }

        public async Task UpdateParticipantAsync(ActivityParticipant participant)
        {
            _context.ActivityParticipants.Update(participant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateActivityAsync(Activity activity)
        {
            _context.Activities.Update(activity);
            await _context.SaveChangesAsync();
        }
    }
}
