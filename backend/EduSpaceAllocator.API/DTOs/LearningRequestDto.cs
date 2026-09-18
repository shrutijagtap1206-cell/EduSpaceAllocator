using System.ComponentModel.DataAnnotations;

namespace EduSpaceAllocator.API.DTOs;

public class LearningRequestDto
{
    public int RequestId { get; set; }

    [Required]
    [MinLength(2)]
    public string Organization { get; set; } = string.Empty;

    [Required]
    [MinLength(2)]
    public string ProgramType { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int StudentCapacity { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Budget { get; set; }

    [Required]
    [MinLength(2)]
    public string PreferredLocation { get; set; } = string.Empty;

    [Range(-90, 90)]
    public double PreferredLatitude { get; set; }

    [Range(-180, 180)]
    public double PreferredLongitude { get; set; }

    [Range(1, int.MaxValue)]
    public int CommunityId { get; set; }
}
