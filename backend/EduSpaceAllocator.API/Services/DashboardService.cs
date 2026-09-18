using EduSpaceAllocator.API.Data;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Services;

public class DashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<object> GetMetricsAsync()
    {
        var spaces = await _db.Spaces
            .AsNoTracking()
            .ToListAsync();

        var requests = await _db.LearningRequests
            .AsNoTracking()
            .ToListAsync();

        var communities = await _db.Communities
            .AsNoTracking()
            .ToListAsync();

        var allocations = await _db.Allocations
            .AsNoTracking()
            .ToListAsync();

        var partners = await _db.Partners
            .AsNoTracking()
            .ToListAsync();

        var courses = await _db.Courses
            .AsNoTracking()
            .ToListAsync();

        var schedules = await _db.Schedules
            .AsNoTracking()
            .ToListAsync();

        return new
        {
            availableSpaces = spaces.Count(s => s.Availability),
            totalSpaces = spaces.Count,
            learningRequests = requests.Count,
            communities = communities.Count,
            matchedAllocations = allocations.Count,
            studentsServed = allocations.Sum(a => a.StudentsServed),
            partners = partners.Count,
            courses = courses.Count,
            scheduledSessions = schedules.Count,

            averageOptimizationScore =
                allocations.Count == 0
                    ? 0
                    : Math.Round(
                        allocations.Average(a => a.OptimizationScore) * 100,
                        1),

            averageDistanceKm =
                allocations.Count == 0
                    ? 0
                    : Math.Round(
                        allocations.Average(a => a.DistanceKm),
                        2),

            averageCapacityUtilization =
                allocations.Count == 0
                    ? 0
                    : Math.Round(
                        allocations.Average(a => a.CapacityUtilization) * 100,
                        1)
        };
    }
}
