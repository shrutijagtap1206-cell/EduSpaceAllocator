namespace EduSpaceAllocator.API.DTOs;

public class SpaceDto
{
    public int SpaceId { get; set; }
    public string BuildingName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double FloorArea { get; set; }
    public int Capacity { get; set; }
    public decimal RentalCost { get; set; }
    public bool Availability { get; set; }
}
