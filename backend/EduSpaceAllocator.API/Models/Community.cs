using System.ComponentModel.DataAnnotations;

namespace EduSpaceAllocator.API.Models;

public class Community
{
    [Key]
    public int CommunityId { get; set; }

    public int Population { get; set; }

    public int StudentCount { get; set; }

    public double LiteracyRate { get; set; }

    public string IncomeGroup { get; set; } = string.Empty;
}
