using EduSpaceAllocator.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EduSpaceAllocator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SocialImpactController : ControllerBase
{
    private readonly SocialImpactService _service;

    public SocialImpactController(SocialImpactService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _service.GetMetricsAsync());
    }
}
