using EduSpaceAllocator.API.DTOs;
using EduSpaceAllocator.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EduSpaceAllocator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpacesController : ControllerBase
{
    private readonly SpaceService _spaceService;

    public SpacesController(SpaceService spaceService)
    {
        _spaceService = spaceService;
    }

    // GET: api/spaces
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SpaceDto>>> GetSpaces()
    {
        var spaces = await _spaceService.GetAllAsync();
        return Ok(spaces);
    }

    // GET: api/spaces/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SpaceDto>> GetSpace(int id)
    {
        var space = await _spaceService.GetByIdAsync(id);

        if (space == null)
        {
            return NotFound(new
            {
                message = "Space not found",
                spaceId = id
            });
        }

        return Ok(space);
    }

    // POST: api/spaces
    [HttpPost]
    public async Task<ActionResult<SpaceDto>> CreateSpace(SpaceDto dto)
    {
        var createdSpace = await _spaceService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetSpace),
            new { id = createdSpace.SpaceId },
            createdSpace);
    }

    // PUT: api/spaces/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpace(int id, SpaceDto dto)
    {
        if (id != dto.SpaceId)
        {
            return BadRequest(new
            {
                message = "URL id and SpaceId must match"
            });
        }

        var updated = await _spaceService.UpdateAsync(id, dto);

        if (!updated)
        {
            return NotFound(new
            {
                message = "Space not found",
                spaceId = id
            });
        }

        return NoContent();
    }

    // DELETE: api/spaces/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpace(int id)
    {
        var deleted = await _spaceService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Space not found",
                spaceId = id
            });
        }

        return NoContent();
    }
}
