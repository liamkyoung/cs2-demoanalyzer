using CSProsLibrary.Models;
using CSProsLibrary.Models.Dtos;
using CSProsLibrary.Repositories.Interfaces;
using CSProsLibrary.Services.Interfaces;
using CSProsLibrary.Services.Scraping.Pages;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace CSProsLibrary.Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _teamRepository;
    private readonly ILogger<ITeamService> _logger;
    private readonly ICountryService _countryService;

    public TeamService(ITeamRepository teamRepository, ICountryService countryService, ILogger<ITeamService> logger)
    {
        _teamRepository = teamRepository;
        _countryService = countryService;
        _logger = logger;
    }

    public async Task<Team?> GetTeamById(int id)
    {
        return await _teamRepository.GetByIdAsync(id);
    }

    public async Task<Team?> GetTeamByName(string teamName)
    {
        try
        {
            var team = await _teamRepository.GetByNameAsync(teamName);

            if (team != null)
            {
                return team;
            }
        }
        catch (Exception e) { }

        _logger.LogError($"Could not find team from name: {teamName}");
        return null;
    }

    public async Task<Team?> GenerateTeamFromParsedData(TeamProfileDto parsedTeamData)
    {
        Country? country = null;
        try
        {
            _logger.LogInformation($"Could not find team by name in DB: {parsedTeamData.Name}");
            
            country = await _countryService.GetCountryByName(parsedTeamData.CountryName);
      
            if (country == null)
            {
                _logger.LogError("Could not find country.");
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"Could not generate team. {e.Message}");
            return null;
        }
        
        return new Team()
        {
            Name = parsedTeamData.Name,
            CoachName = parsedTeamData.CoachName,
            Country = country,
            HltvProfile = parsedTeamData.HltvProfile,
            HltvRanking = parsedTeamData.HltvRanking,
            ImageSrc = parsedTeamData.ImageSrc,
            TimeSinceLastUpdated = DateTimeOffset.UtcNow
        };
    }

    public async Task<bool> AddTeamAsync(Team team)
    {
        try
        {
            await _teamRepository.AddAsync(team);
            _logger.LogInformation($"Added team to DB {team.Name}");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError($"EXCEPTION: Couldn't add team to DB. {e.Message}");
        }

        return false;
    }

    public async Task<bool> UpdateTeamAsync(TeamProfileDto teamProfile)
    {
        var team = await _teamRepository.GetByNameAsync(teamProfile.Name);
        if (team == null)
        {
            return false;
        }

        return await _teamRepository.UpdateAsync(team);
    }
    
    public async Task<Team?> AddTeamAsync(TeamProfileDto teamProfile)
    {
        var team = await GenerateTeamFromParsedData(teamProfile);

        if (team == null)
        {
            return null;
        }
        
        if (await _teamRepository.AddAsync(team))
        {
            return team;
        }

        return null;
    }

    public async Task<IEnumerable<Player>> GetPlayersOnTeamById(int teamId)
    {
        return await _teamRepository.GetPlayersOnTeamById(teamId);
    }

    public async Task<IEnumerable<Team>> GetAllTeamProfiles()
    {
        return await _teamRepository.GetAllTeamProfiles();
    }
}