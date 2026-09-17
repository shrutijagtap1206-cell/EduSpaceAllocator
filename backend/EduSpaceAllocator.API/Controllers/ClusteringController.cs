using System.Net.Http.Json;
using EduSpaceAllocator.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClusteringController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;

    public ClusteringController(
        AppDbContext context,
        IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient("AIEngine");
    }

    [HttpPost("run")]
    public async Task<IActionResult> RunClustering(
        [FromQuery] int nClusters = 3)
    {
        if (nClusters < 2)
        {
            return BadRequest(new
            {
                message = "nClusters must be at least 2."
            });
        }

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

        if (communities.Count < nClusters)
        {
            return BadRequest(new
            {
                message = $"At least {nClusters} communities are required.",
                availableCommunities = communities.Count
            });
        }

        var payload = new
        {
            n_clusters = nClusters,
            communities
        };

        var response = await _httpClient.PostAsJsonAsync(
            "/cluster",
            payload);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            return StatusCode(
                (int)response.StatusCode,
                new
                {
                    message = "AI engine returned an error.",
                    details = error
                });
        }

        var result = await response.Content.ReadFromJsonAsync<object>();

        return Ok(result);
    }
}
