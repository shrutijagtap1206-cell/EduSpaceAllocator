using System.ComponentModel.DataAnnotations;

namespace EduSpaceAllocator.API.DTOs;

public class SpaceDto
{
    public int SpaceId { get; set; }

    [Required]
    [MinLength(2)]
    public string BuildingName { get; set; } = string.Empty;

    [Required]
    [MinLength(3)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [MinLength(2)]
    public string City { get; set; } = string.Empty;

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }

    [Range(1, double.MaxValue)]
    public double FloorArea { get; set; }

    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal RentalCost { get; set; }

    public bool Availability { get; set; }
}
