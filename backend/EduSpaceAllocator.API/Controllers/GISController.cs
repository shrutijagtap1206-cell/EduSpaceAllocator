using EduSpaceAllocator.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace EduSpaceAllocator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GISController : ControllerBase
{
    private readonly AppDbContext _context;

    public GISController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("distance")]
    public async Task<IActionResult> GetDistance(int fromId, int toId)
    {
        var fromSpace = await _context.Spaces.FindAsync(fromId);
        var toSpace = await _context.Spaces.FindAsync(toId);

        if (fromSpace == null || toSpace == null)
        {
            return NotFound(new
            {
                message = "One or both spaces were not found."
            });
        }

        double distanceKm = CalculateHaversineDistance(
            fromSpace.Latitude,
            fromSpace.Longitude,
            toSpace.Latitude,
            toSpace.Longitude
        );

        return Ok(new
        {
            fromId = fromSpace.SpaceId,
            toId = toSpace.SpaceId,
            fromSpace = fromSpace.BuildingName,
            toSpace = toSpace.BuildingName,
            distanceKm = Math.Round(distanceKm, 2)
        });
    }

    private static double CalculateHaversineDistance(
        double lat1,
        double lon1,
        double lat2,
        double lon2)
    {
        const double earthRadiusKm = 6371.0;

        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);

        double a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(DegreesToRadians(lat1)) *
            Math.Cos(DegreesToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
