using CSProsLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GeneralController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public GeneralController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }
    
    [HttpGet("AppStats")]
    public async Task<IActionResult> AppStats()
    {
        var dto = await _analyticsService.GetAppStats();
        return Ok(dto);
    }
}