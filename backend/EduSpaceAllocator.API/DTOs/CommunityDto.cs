namespace EduSpaceAllocator.API.DTOs;

public class CommunityDto
{
    public int CommunityId { get; set; }
    public int Population { get; set; }
    public int StudentCount { get; set; }
    public double LiteracyRate { get; set; }
    public string IncomeGroup { get; set; } = string.Empty;
}
