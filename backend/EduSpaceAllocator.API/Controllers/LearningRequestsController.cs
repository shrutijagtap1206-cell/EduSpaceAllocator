using EduSpaceAllocator.API.DTOs;
using EduSpaceAllocator.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EduSpaceAllocator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LearningRequestsController : ControllerBase
{
    private readonly LearningRequestService _service;

    public LearningRequestsController(LearningRequestService service)
    {
        _service = service;
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
