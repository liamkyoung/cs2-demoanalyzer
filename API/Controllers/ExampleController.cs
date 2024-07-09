using CSProsLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExampleController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public ExampleController(IPlayerService playerService)
    {
        _playerService = playerService;
    }
    
    [HttpGet("Test")]
    public async Task<IActionResult> Test()
    {
        return Ok();
    }
}