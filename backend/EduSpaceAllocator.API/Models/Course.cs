using System.ComponentModel.DataAnnotations;

namespace EduSpaceAllocator.API.Models;

public class Course
{
    [Key]
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int RequiredCapacity { get; set; }
    public int DurationHours { get; set; }
    public string SkillLevel { get; set; } = string.Empty;
}
