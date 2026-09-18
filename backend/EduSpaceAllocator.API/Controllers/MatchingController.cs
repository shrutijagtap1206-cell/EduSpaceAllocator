using System.Net.Http.Json;
using EduSpaceAllocator.API.Data;
using EduSpaceAllocator.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchingController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;

    public MatchingController(
        AppDbContext context,
        IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient("AIEngine");
    }

    [HttpPost("run")]
    public async Task<IActionResult> RunMatching()
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

        var payload = new
        {
            spaces,
            requests,
            communities
        };

        var response = await _httpClient.PostAsJsonAsync(
            "/matching",
            payload);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            return StatusCode(
                (int)response.StatusCode,
                new
                {
                    message = "AI engine matching failed.",
                    details = error
                });
        }

        var result = await response.Content.ReadFromJsonAsync<object>();

        return Ok(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveAllocations()
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

        var payload = new
        {
            spaces,
            requests,
            communities
        };

        var response = await _httpClient.PostAsJsonAsync(
            "/matching",
            payload);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode(
                (int)response.StatusCode,
                new
                {
                    message = "AI engine matching failed."
                });
        }

        var result = await response.Content
            .ReadFromJsonAsync<MatchingResponse>();

        if (result == null)
        {
            return BadRequest(new
            {
                message = "Invalid matching response."
            });
        }

        foreach (var match in result.Matches)
        {
            var existing = await _context.Allocations
                .FirstOrDefaultAsync(a =>
                    a.SpaceId == match.SpaceId &&
                    a.RequestId == match.RequestId);

            if (existing != null)
            {
                existing.MatchScore = match.MatchScore;
                existing.DistanceKm = match.DistanceKm;
                existing.CapacityUtilization = match.CapacityUtilization;
                existing.CostEfficiency = match.CostEfficiency;
                existing.StudentReach = match.StudentReach;
                existing.StudentsServed = match.StudentsServed;
                existing.OptimizationScore = match.OptimizationScore;
                existing.Status = "Recommended";
            }
            else
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
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Allocations saved successfully.",
            savedCount = result.Matches.Count,
            summary = result.Summary
        });
    }

    [HttpGet("allocations")]
    public async Task<IActionResult> GetAllocations()
    {
        var allocations = await _context.Allocations
            .AsNoTracking()
            .OrderByDescending(a => a.OptimizationScore)
            .Select(a => new
            {
                a.AllocationId,
                a.SpaceId,
                a.RequestId,
                a.MatchId,
                a.DistanceKm,
                a.MatchScore,
                a.CapacityUtilization,
                a.CostEfficiency,
                a.StudentReach,
                a.StudentsServed,
                a.OptimizationScore,
                a.Status
            })
            .ToListAsync();

        return Ok(allocations);
    }

    private class MatchingResponse
    {
        public int CandidateCount { get; set; }

        public int MatchCount { get; set; }

        public List<MatchingResult> Matches { get; set; } = [];

        public MatchingSummary Summary { get; set; } = new();
    }

    private class MatchingResult
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

    private class MatchingSummary
    {
        public int TotalStudentsServed { get; set; }

        public double AverageDistanceKm { get; set; }

        public double AverageCapacityUtilization { get; set; }

        public double AverageCostEfficiency { get; set; }

        public double AverageStudentReach { get; set; }

        public double OverallOptimizationScore { get; set; }
    }
}
