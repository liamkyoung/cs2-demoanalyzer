using Microsoft.AspNetCore.Mvc;
using CSProsLibrary.Models.Dtos.Viewmodels;
using CSProsLibrary.Services.Interfaces;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayerController : ControllerBase
{
    private readonly IPlayerService _playerService;
    private readonly ITeamService _teamService;
    
    public PlayerController(IPlayerService playerService, ITeamService teamService)
    {
        _playerService = playerService;
        _teamService = teamService;
    }
    
    [HttpGet("GetPlayer")]
    public async Task<IActionResult> GetPlayer(string playerName)
    {
        var player = await _playerService.GetPlayerByGamerTag(playerName);

        if (player == null)
        {
            return NotFound("Player not found");
        }
        
        return Ok(PlayerProfile.Convert(player));
    }
    
    [HttpGet("GetPlayerTeam")]
    public async Task<IActionResult> GetTeamByPlayerName(string playerName)
    {
        var player = await _playerService.GetPlayerByGamerTag(playerName);

        if (player == null)
        {
            return NotFound("Player not found.");
        }

        if (player.TeamId == null)
        {
            return NotFound("Player does not have a team.");
        }

        var team = await _teamService.GetTeamById(player.TeamId.Value);
        
        return Ok(team);
    }
    
    
    [HttpGet("GetTeammates")]
    public async Task<IActionResult> GetPlayerTeammates(string playerName)
    {
        var player = await _playerService.GetPlayerByGamerTag(playerName);

        if (player == null)
        {
            return NotFound("Player not found...");
        }
        
        var teammates = await _playerService.GetTeammates(player);
        
        return Ok(teammates.Select(p => PlayerProfile.Convert(p)));
    }
    
    [HttpGet("TrendingPlayers")]
    public async Task<IActionResult> GetTrendingPlayers()
    {
        var players = await _playerService.GetTrendingPlayers(TimeSpan.FromDays(30), 4);

        var playerProfiles = players.Select(p => PlayerProfile.Convert(p));
        
        return Ok(playerProfiles);
    }
    
    [HttpGet("GetAllTeams")]
    public async Task<IActionResult> GetAllTeams()
    {
        var teams = await _teamService.GetAllTeamProfiles();
        
        return Ok(teams.Select(team => TeamProfile.Convert(team)));
    }
}