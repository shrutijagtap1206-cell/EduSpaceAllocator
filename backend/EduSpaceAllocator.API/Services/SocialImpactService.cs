using EduSpaceAllocator.API.Data;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Services;

public class SocialImpactService
{
    private readonly AppDbContext _db;

    public SocialImpactService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<object> GetMetricsAsync()
    {
        var allocations = await _db.Allocations
            .AsNoTracking()
            .ToListAsync();

        var schedules = await _db.Schedules
            .AsNoTracking()
            .Include(s => s.Course)
            .ToListAsync();

        var communitiesReached = await _db.Allocations
            .AsNoTracking()
            .Include(a => a.LearningRequest)
            .Select(a => a.LearningRequest!.CommunityId)
            .Where(id => id > 0)
            .Distinct()
            .CountAsync();

        var spacesReused = allocations
            .Select(a => a.SpaceId)
            .Distinct()
            .Count();

        var studentsServed = allocations.Sum(a => a.StudentsServed);

        var scheduledHours = schedules
            .Sum(s =>
            {
                if (!TimeSpan.TryParse(s.StartTime, out var start))
                    return 0;

                if (!TimeSpan.TryParse(s.EndTime, out var end))
                    return 0;

                return (int)Math.Max(0, (end - start).TotalHours);
            });

        return new
        {
            studentsServed,
            learningHoursDelivered = scheduledHours,
            communitiesReached,
            spacesReused,
            scheduledSessions = schedules.Count,
            totalAllocations = allocations.Count
        };
    }
}
