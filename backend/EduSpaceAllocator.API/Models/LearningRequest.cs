using System.ComponentModel.DataAnnotations;

namespace EduSpaceAllocator.API.Models;

public class LearningRequest
{
    [Key]
    public int RequestId { get; set; }

    public string Organization { get; set; } = string.Empty;

    public string ProgramType { get; set; } = string.Empty;

    public int StudentCapacity { get; set; }

    public decimal Budget { get; set; }

    public string PreferredLocation { get; set; } = string.Empty;
}
