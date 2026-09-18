using EduSpaceAllocator.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EduSpaceAllocator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AllocationsController : ControllerBase
{
    private readonly AllocationService _service;

    public AllocationsController(AllocationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var allocations = await _service.GetAllAsync();

        return Ok(allocations);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var allocation = await _service.GetByIdAsync(id);

        if (allocation == null)
        {
            return NotFound(new
            {
                message = "Allocation not found."
            });
        }

        return Ok(allocation);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromQuery] string status)
    {
        var allowedStatuses = new[]
        {
            "Recommended",
            "Approved",
            "Rejected",
            "Completed"
        };

        if (!allowedStatuses.Contains(
                status,
                StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                message = "Invalid status.",
                allowedStatuses
            });
        }

        var updated = await _service.UpdateStatusAsync(
            id,
            status);

        if (!updated)
        {
            return NotFound(new
            {
                message = "Allocation not found."
            });
        }

        return Ok(new
        {
            message = "Allocation status updated.",
            allocationId = id,
            status
        });
    }
}