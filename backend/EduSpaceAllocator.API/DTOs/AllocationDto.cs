namespace EduSpaceAllocator.API.DTOs;

public class AllocationDto
{
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

    public string Status { get; set; } = string.Empty;
}
