using System.ComponentModel.DataAnnotations;

namespace EduSpaceAllocator.API.Models;

public class Partner
{
    [Key]
    public int PartnerId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string PartnerType { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
}
