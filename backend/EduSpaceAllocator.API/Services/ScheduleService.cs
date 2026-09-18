using EduSpaceAllocator.API.Data;
using EduSpaceAllocator.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Services;

public class ScheduleService
{
    private readonly AppDbContext _db;
    private readonly AuditService _audit;

    public ScheduleService(AppDbContext db, AuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    public async Task<List<Schedule>> GetAllAsync()
    {
        return await _db.Schedules
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.Allocation)
                .ThenInclude(a => a!.Space)
            .OrderBy(s => s.SessionDate)
            .ThenBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<Schedule> CreateAsync(Schedule schedule)
    {
        ValidateBasic(schedule);

        schedule.SessionDate = DateTime.SpecifyKind(
            schedule.SessionDate.Date,
            DateTimeKind.Utc);

        var allocation = await _db.Allocations
            .Include(a => a.Space)
            .FirstOrDefaultAsync(a => a.AllocationId == schedule.AllocationId);

        if (allocation == null)
            throw new ArgumentException("Allocation not found.");

        var course = await _db.Courses
            .FirstOrDefaultAsync(c => c.CourseId == schedule.CourseId);

        if (course == null)
            throw new ArgumentException("Course not found.");

        if (allocation.Space == null)
            throw new ArgumentException("Allocated space not found.");

        if (course.RequiredCapacity > allocation.Space.Capacity)
            throw new ArgumentException(
                $"Course requires {course.RequiredCapacity} seats, but the allocated space has only {allocation.Space.Capacity}.");

        if (await HasConflictAsync(schedule))
            throw new ArgumentException(
                "Schedule conflict: another session already uses this allocation during the selected time.");

        schedule.Status = string.IsNullOrWhiteSpace(schedule.Status)
            ? "Planned"
            : schedule.Status;

        schedule.SessionDate = DateTime.SpecifyKind(
            schedule.SessionDate.Date,
            DateTimeKind.Utc);

        _db.Schedules.Add(schedule);
        await _db.SaveChangesAsync();

        await _audit.LogAsync(
            "CREATE",
            "Schedule",
            schedule.ScheduleId.ToString(),
            $"Course {course.CourseName} scheduled for allocation {schedule.AllocationId} on {schedule.SessionDate:yyyy-MM-dd} from {schedule.StartTime} to {schedule.EndTime}.");

        return schedule;
    }

    public async Task<Schedule?> UpdateAsync(int id, Schedule updated)
    {
        ValidateBasic(updated);

        updated.SessionDate = DateTime.SpecifyKind(
            updated.SessionDate.Date,
            DateTimeKind.Utc);

        var schedule = await _db.Schedules
            .FirstOrDefaultAsync(s => s.ScheduleId == id);

        if (schedule == null)
            return null;

        var allocation = await _db.Allocations
            .Include(a => a.Space)
            .FirstOrDefaultAsync(a => a.AllocationId == updated.AllocationId);

        if (allocation == null)
            throw new ArgumentException("Allocation not found.");

        var course = await _db.Courses
            .FirstOrDefaultAsync(c => c.CourseId == updated.CourseId);

        if (course == null)
            throw new ArgumentException("Course not found.");

        if (allocation.Space == null)
            throw new ArgumentException("Allocated space not found.");

        if (course.RequiredCapacity > allocation.Space.Capacity)
            throw new ArgumentException(
                $"Course requires {course.RequiredCapacity} seats, but the allocated space has only {allocation.Space.Capacity}.");

        if (await HasConflictAsync(updated, id))
            throw new ArgumentException(
                "Schedule conflict: another session already uses this allocation during the selected time.");

        schedule.AllocationId = updated.AllocationId;
        schedule.CourseId = updated.CourseId;
        schedule.SessionDate = DateTime.SpecifyKind(
            updated.SessionDate.Date,
            DateTimeKind.Utc);
        schedule.StartTime = updated.StartTime;
        schedule.EndTime = updated.EndTime;
        schedule.Status = string.IsNullOrWhiteSpace(updated.Status)
            ? "Planned"
            : updated.Status;

        await _db.SaveChangesAsync();

        await _audit.LogAsync(
            "UPDATE",
            "Schedule",
            id.ToString(),
            $"Schedule updated for course {course.CourseName} on {schedule.SessionDate:yyyy-MM-dd}.");

        return schedule;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var schedule = await _db.Schedules.FindAsync(id);

        if (schedule == null)
            return false;

        _db.Schedules.Remove(schedule);
        await _db.SaveChangesAsync();

        await _audit.LogAsync(
            "DELETE",
            "Schedule",
            id.ToString(),
            $"Schedule {id} deleted.");

        return true;
    }

    private async Task<bool> HasConflictAsync(
        Schedule candidate,
        int? excludeId = null)
    {
        if (!TimeSpan.TryParse(candidate.StartTime, out var candidateStart))
            return false;

        if (!TimeSpan.TryParse(candidate.EndTime, out var candidateEnd))
            return false;

        var existingSchedules = await _db.Schedules
            .AsNoTracking()
            .Where(s =>
                s.AllocationId == candidate.AllocationId &&
                s.SessionDate.Date == candidate.SessionDate.Date &&
                (!excludeId.HasValue || s.ScheduleId != excludeId.Value))
            .ToListAsync();

        foreach (var existing in existingSchedules)
        {
            if (!TimeSpan.TryParse(existing.StartTime, out var existingStart))
                continue;

            if (!TimeSpan.TryParse(existing.EndTime, out var existingEnd))
                continue;

            if (existingStart < candidateEnd &&
                existingEnd > candidateStart)
            {
                return true;
            }
        }

        return false;
    }

    private static void ValidateBasic(Schedule schedule)
    {
        if (schedule.AllocationId <= 0)
            throw new ArgumentException("Allocation is required.");

        if (schedule.CourseId <= 0)
            throw new ArgumentException("Course is required.");

        if (schedule.SessionDate.Date < DateTime.UtcNow.Date)
            throw new ArgumentException("Session date cannot be in the past.");

        if (!TimeSpan.TryParse(schedule.StartTime, out var start))
            throw new ArgumentException("Start time must be valid, for example 10:00.");

        if (!TimeSpan.TryParse(schedule.EndTime, out var end))
            throw new ArgumentException("End time must be valid, for example 12:00.");

        if (end <= start)
            throw new ArgumentException("End time must be later than start time.");

        if (end - start > TimeSpan.FromHours(12))
            throw new ArgumentException("A session cannot be longer than 12 hours.");
    }
}



