using EduSpaceAllocator.API.Data;
using EduSpaceAllocator.API.DTOs;
using EduSpaceAllocator.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LearningRequestsController : ControllerBase
{
    private readonly LearningRequestService _service;
    private readonly AppDbContext _context;

    public LearningRequestsController(
        LearningRequestService service,
        AppDbContext context)
    {
        _service = service;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LearningRequestDto>>> GetRequests()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LearningRequestDto>> GetRequest(int id)
    {
        var request = await _service.GetByIdAsync(id);

        if (request == null)
            return NotFound(new
            {
                message = "Learning request not found",
                requestId = id
            });

        return Ok(request);
    }

    [HttpPost]
    public async Task<ActionResult<LearningRequestDto>> CreateRequest(
        LearningRequestDto dto)
    {
        if (dto.StudentCapacity <= 0)
            return BadRequest(new
            {
                message = "StudentCapacity must be greater than 0."
            });

        if (dto.Budget <= 0)
            return BadRequest(new
            {
                message = "Budget must be greater than 0."
            });

        if (dto.PreferredLatitude < -90 || dto.PreferredLatitude > 90)
            return BadRequest(new
            {
                message = "PreferredLatitude must be between -90 and 90."
            });

        if (dto.PreferredLongitude < -180 || dto.PreferredLongitude > 180)
            return BadRequest(new
            {
                message = "PreferredLongitude must be between -180 and 180."
            });

        var communityExists = await _context.Communities
            .AnyAsync(c => c.CommunityId == dto.CommunityId);

        if (!communityExists)
            return BadRequest(new
            {
                message = "CommunityId does not exist.",
                communityId = dto.CommunityId
            });

        var created = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetRequest),
            new { id = created.RequestId },
            created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRequest(
        int id,
        LearningRequestDto dto)
    {
        if (id != dto.RequestId)
            return BadRequest(new
            {
                message = "URL id and RequestId must match"
            });

        var communityExists = await _context.Communities
            .AnyAsync(c => c.CommunityId == dto.CommunityId);

        if (!communityExists)
            return BadRequest(new
            {
                message = "CommunityId does not exist.",
                communityId = dto.CommunityId
            });

        var updated = await _service.UpdateAsync(id, dto);

        if (!updated)
            return NotFound(new
            {
                message = "Learning request not found",
                requestId = id
            });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRequest(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
            return NotFound(new
            {
                message = "Learning request not found",
                requestId = id
            });

        return NoContent();
    }
}
