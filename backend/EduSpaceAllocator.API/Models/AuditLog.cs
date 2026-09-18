using System.ComponentModel.DataAnnotations;

namespace EduSpaceAllocator.API.Models;

public class AuditLog
{
    [Key]
    public int AuditLogId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
