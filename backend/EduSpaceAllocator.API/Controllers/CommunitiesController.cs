using EduSpaceAllocator.API.DTOs;
using EduSpaceAllocator.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EduSpaceAllocator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommunitiesController : ControllerBase
{
    private readonly CommunityService _service;

    public CommunitiesController(CommunityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommunityDto>>> GetCommunities()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommunityDto>> GetCommunity(int id)
    {
        var community = await _service.GetByIdAsync(id);

        if (community == null)
        {
            return NotFound(new
            {
                message = "Community not found",
                communityId = id
            });
        }

        return Ok(community);
    }

    [HttpPost]
    public async Task<ActionResult<CommunityDto>> CreateCommunity(
        CommunityDto dto)
    {
        if (dto.Population <= 0)
        {
            return BadRequest(new
            {
                message = "Population must be greater than 0."
            });
        }

        if (dto.StudentCount < 0 || dto.StudentCount > dto.Population)
        {
            return BadRequest(new
            {
                message = "StudentCount must be between 0 and Population."
            });
        }

        if (dto.LiteracyRate < 0 || dto.LiteracyRate > 100)
        {
            return BadRequest(new
            {
                message = "LiteracyRate must be between 0 and 100."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.IncomeGroup))
        {
            return BadRequest(new
            {
                message = "IncomeGroup is required."
            });
        }

        var allowedIncomeGroups = new[]
        {
            "Low",
            "Medium",
            "High"
        };

        if (!allowedIncomeGroups.Contains(
                dto.IncomeGroup.Trim(),
                StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                message = "IncomeGroup must be Low, Medium, or High."
            });
        }

        var created = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetCommunity),
            new { id = created.CommunityId },
            created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCommunity(
        int id,
        CommunityDto dto)
    {
        if (id != dto.CommunityId)
        {
            return BadRequest(new
            {
                message = "URL id and CommunityId must match."
            });
        }

        var updated = await _service.UpdateAsync(id, dto);

        if (!updated)
        {
            return NotFound(new
            {
                message = "Community not found",
                communityId = id
            });
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCommunity(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Community not found",
                communityId = id
            });
        }

        return NoContent();
    }
}
