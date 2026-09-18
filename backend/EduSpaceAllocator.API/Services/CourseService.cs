using EduSpaceAllocator.API.Data;
using EduSpaceAllocator.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Services;

public class CourseService
{
    private readonly AppDbContext _db;

    public CourseService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Course>> GetAllAsync()
    {
        return await _db.Courses
            .AsNoTracking()
            .OrderBy(c => c.CourseId)
            .ToListAsync();
    }

    public async Task<Course> CreateAsync(Course course)
    {
        Validate(course);

        _db.Courses.Add(course);
        await _db.SaveChangesAsync();

        return course;
    }

    public async Task<Course?> UpdateAsync(int id, Course updated)
    {
        Validate(updated);

        var course = await _db.Courses.FindAsync(id);

        if (course == null)
            return null;

        course.CourseName = updated.CourseName;
        course.Category = updated.Category;
        course.RequiredCapacity = updated.RequiredCapacity;
        course.DurationHours = updated.DurationHours;
        course.SkillLevel = updated.SkillLevel;

        await _db.SaveChangesAsync();

        return course;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var course = await _db.Courses.FindAsync(id);

        if (course == null)
            return false;

        _db.Courses.Remove(course);
        await _db.SaveChangesAsync();

        return true;
    }

    private static void Validate(Course course)
    {
        if (string.IsNullOrWhiteSpace(course.CourseName))
            throw new ArgumentException("Course name is required.");

        if (string.IsNullOrWhiteSpace(course.Category))
            throw new ArgumentException("Category is required.");

        if (course.RequiredCapacity <= 0)
            throw new ArgumentException("Required capacity must be greater than zero.");

        if (course.DurationHours <= 0)
            throw new ArgumentException("Duration must be greater than zero.");

        if (string.IsNullOrWhiteSpace(course.SkillLevel))
            throw new ArgumentException("Skill level is required.");
    }
}
