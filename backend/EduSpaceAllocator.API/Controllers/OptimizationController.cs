using System.Net.Http.Json;
using EduSpaceAllocator.API.Data;
using EduSpaceAllocator.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OptimizationController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;

    public OptimizationController(
        AppDbContext context,
        IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient("AIEngine");
    }

    [HttpPost("run")]
    public async Task<IActionResult> RunOptimization()
    {
        var spaces = await _context.Spaces
            .AsNoTracking()
            .Where(s => s.Availability)
            .Select(s => new
            {
                spaceId = s.SpaceId,
                buildingName = s.BuildingName,
                address = s.Address,
                latitude = s.Latitude,
                longitude = s.Longitude,
                capacity = s.Capacity,
                rentalCost = s.RentalCost,
                availability = s.Availability
            })
            .ToListAsync();

        var requests = await _context.LearningRequests
            .AsNoTracking()
            .Select(r => new
            {
                requestId = r.RequestId,
                organization = r.Organization,
                programType = r.ProgramType,
                studentCapacity = r.StudentCapacity,
                budget = r.Budget,
                preferredLocation = r.PreferredLocation,
                preferredLatitude = r.PreferredLatitude,
                preferredLongitude = r.PreferredLongitude,
                communityId = r.CommunityId
            })
            .ToListAsync();

        var communities = await _context.Communities
            .AsNoTracking()
            .Select(c => new
            {
                communityId = c.CommunityId,
                population = c.Population,
                studentCount = c.StudentCount,
                literacyRate = c.LiteracyRate,
                incomeGroup = c.IncomeGroup
            })
            .ToListAsync();

        if (spaces.Count == 0)
        {
            return BadRequest(new
            {
                message = "No available spaces found."
            });
        }

        if (requests.Count == 0)
        {
            return BadRequest(new
            {
                message = "No learning requests found."
            });
        }

        var response = await _httpClient.PostAsJsonAsync(
            "/optimization",
            new
            {
                spaces,
                requests,
                communities
            });

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            return StatusCode(
                (int)response.StatusCode,
                new
                {
                    message = "AI engine optimization failed.",
                    details = error
                });
        }

        var result =
            await response.Content.ReadFromJsonAsync<OptimizationResponse>();

        if (result == null)
        {
            return StatusCode(
                502,
                new
                {
                    message = "AI engine returned an empty response."
                });
        }

        foreach (var match in result.Matches)
        {
            var existing = await _context.Allocations
                .FirstOrDefaultAsync(a => a.MatchId == match.MatchId);

            if (existing == null)
            {
                _context.Allocations.Add(new Allocation
                {
                    SpaceId = match.SpaceId,
                    RequestId = match.RequestId,
                    MatchId = match.MatchId,
                    DistanceKm = match.DistanceKm,
                    MatchScore = match.MatchScore,
                    CapacityUtilization = match.CapacityUtilization,
                    CostEfficiency = match.CostEfficiency,
                    StudentReach = match.StudentReach,
                    StudentsServed = match.StudentsServed,
                    OptimizationScore = match.OptimizationScore,
                    Status = "Recommended"
                });
            }
            else
            {
                existing.DistanceKm = match.DistanceKm;
                existing.MatchScore = match.MatchScore;
                existing.CapacityUtilization = match.CapacityUtilization;
                existing.CostEfficiency = match.CostEfficiency;
                existing.StudentReach = match.StudentReach;
                existing.StudentsServed = match.StudentsServed;
                existing.OptimizationScore = match.OptimizationScore;
            }
        }

        await _context.SaveChangesAsync();

        return Ok(result);
    }

    private sealed class OptimizationResponse
    {
        public int CandidateCount { get; set; }
        public int MatchCount { get; set; }
        public List<OptimizationMatch> Matches { get; set; } = new();
        public OptimizationSummary Summary { get; set; } = new();
    }

    private sealed class OptimizationMatch
    {
        public string MatchId { get; set; } = string.Empty;
        public int SpaceId { get; set; }
        public int RequestId { get; set; }
        public double DistanceKm { get; set; }
        public double MatchScore { get; set; }
        public double CapacityUtilization { get; set; }
        public double CostEfficiency { get; set; }
        public double StudentReach { get; set; }
        public int StudentsServed { get; set; }
        public double OptimizationScore { get; set; }
    }

    private sealed class OptimizationSummary
    {
        public int TotalStudentsServed { get; set; }
        public double AverageDistanceKm { get; set; }
        public double AverageCapacityUtilization { get; set; }
        public double AverageCostEfficiency { get; set; }
        public double AverageStudentReach { get; set; }
        public double OverallOptimizationScore { get; set; }
    }
}
