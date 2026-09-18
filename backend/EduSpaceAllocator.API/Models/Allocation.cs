using System.ComponentModel.DataAnnotations;

namespace EduSpaceAllocator.API.Models;

public class Allocation
{
    [Key]
    public int AllocationId { get; set; }

    public int SpaceId { get; set; }

    public int RequestId { get; set; }

    public string MatchId { get; set; } = string.Empty;

    public double DistanceKm { get; set; }

    public double MatchScore { get; set; }

    public double CapacityUtilization { get; set; }

    public double CostEfficiency { get; set; }

    public double StudentReach { get; set; }

    public int StudentsServed { get; set; }

    public double OptimizationScore { get; set; }

    public string Status { get; set; } = "Recommended";

    public Space? Space { get; set; }

    public LearningRequest? LearningRequest { get; set; }
}
