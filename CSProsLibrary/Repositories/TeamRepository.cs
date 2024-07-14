using System.Linq;
using CSProsLibrary.Models;
using CSProsLibrary.Models.Dtos;
using CSProsLibrary.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CSProsLibrary.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TeamRepository> _logger;

    public TeamRepository(ApplicationDbContext context, ILogger<TeamRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Team?> GetByIdAsync(int id)
    {
        return await _context.Teams.Include(t => t.Country).FirstOrDefaultAsync();
    }

    public async Task<bool> AddAsync(Team team)
    {
        try
        {
            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Added team to DB {team.Name}");
        }
        catch (Exception e)
        {
            _logger.LogInformation($"Could not log team to DB: {team.Name}");
            return false;
        }

        return true;
    }

    public async Task<bool> UpdateAsync(Team updatedTeam)
    {
        try
        {
            var team = await GetByNameAsync(updatedTeam.Name);

            if (team == null)
            {
                _logger.LogInformation($"Could not update team data: {updatedTeam.Name}");
                return false;
            }

            team = updatedTeam;
            
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogInformation($"Exception occurred updating team data: {e.Message}");
        }

        return false;
    }

    public async Task<Team?> GetByNameAsync(string teamName)
    {
        return await _context.Teams.FirstOrDefaultAsync(team => team.Name.ToLower() == teamName.ToLower());
    }

    public async Task<IEnumerable<Team>?> GetTeamsByCountryAsync(Country country)
    {
        return await _context.Teams
            .Where(team => team.Country == country)
            .ToListAsync();
    }

    public async Task<IEnumerable<Team>?> GetTeamsByHLTVRanking(int minimumRanking, int maximumRanking = 0)
    {
        return await _context.Teams
            .Where(team => team.HltvRanking < minimumRanking && team.HltvRanking >= maximumRanking)
            .ToListAsync();
    }

    public async Task<IEnumerable<Player>> GetPlayersOnTeamById(int teamId)
    {
        return await _context.Players.Where(player => player.TeamId == teamId).ToListAsync();
    }

    public async Task<IEnumerable<Team>> GetAllTeamProfiles()
    {
        return await _context.Teams.Include(t => t.Players).ToListAsync();
    }
}