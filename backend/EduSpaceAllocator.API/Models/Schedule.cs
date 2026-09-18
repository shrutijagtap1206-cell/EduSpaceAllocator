using System.ComponentModel.DataAnnotations;

namespace EduSpaceAllocator.API.Models;

public class Schedule
{
    [Key]
    public int ScheduleId { get; set; }
    public int AllocationId { get; set; }
    public int CourseId { get; set; }
    public DateTime SessionDate { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Status { get; set; } = "Planned";

    public Allocation? Allocation { get; set; }
    public Course? Course { get; set; }
}
