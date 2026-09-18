using System.ComponentModel.DataAnnotations;

namespace EduSpaceAllocator.API.Models;

public class SocialImpact
{
    [Key]
    public int SocialImpactId { get; set; }
    public int AllocationId { get; set; }
    public int StudentsServed { get; set; }
    public int HoursDelivered { get; set; }
    public string CommunityOutcome { get; set; } = string.Empty;
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    public Allocation? Allocation { get; set; }
}
