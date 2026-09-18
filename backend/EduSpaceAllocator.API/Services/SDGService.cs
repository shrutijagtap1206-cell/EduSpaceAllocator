using EduSpaceAllocator.API.Data;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Services;

public class SDGService
{
    private readonly AppDbContext _db;

    public SDGService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<object> GetMetricsAsync()
    {
        var allocations = await _db.Allocations
            .AsNoTracking()
            .ToListAsync();

        var communities = await _db.Communities
            .AsNoTracking()
            .ToListAsync();

        var partners = await _db.Partners
            .AsNoTracking()
            .ToListAsync();

        var spaces = await _db.Spaces
            .AsNoTracking()
            .ToListAsync();

        var schedules = await _db.Schedules
            .AsNoTracking()
            .ToListAsync();

        var studentsServed = allocations.Sum(a => a.StudentsServed);

        var spacesReused = allocations
            .Select(a => a.SpaceId)
            .Distinct()
            .Count();

        var communitiesReached = await _db.Allocations
            .AsNoTracking()
            .Include(a => a.LearningRequest)
            .Select(a => a.LearningRequest!.CommunityId)
            .Where(id => id > 0)
            .Distinct()
            .CountAsync();

        return new
        {
            sdg4 = new
            {
                name = "Quality Education",
                indicator = "Students served",
                value = studentsServed
            },

            sdg8 = new
            {
                name = "Decent Work & Economic Growth",
                indicator = "Learning programs scheduled",
                value = schedules.Count
            },

            sdg9 = new
            {
                name = "Industry, Innovation & Infrastructure",
                indicator = "Learning spaces reused",
                value = spacesReused
            },

            sdg10 = new
            {
                name = "Reduced Inequalities",
                indicator = "Communities reached",
                value = communitiesReached
            },

            sdg11 = new
            {
                name = "Sustainable Cities & Communities",
                indicator = "Available spaces",
                value = spaces.Count(s => s.Availability)
            },

            sdg17 = new
            {
                name = "Partnerships for the Goals",
                indicator = "Registered partners",
                value = partners.Count
            }
        };
    }
}
