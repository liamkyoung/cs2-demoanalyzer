using CSProsLibrary.Models;
using CSProsLibrary.Models.Dtos;

namespace CSProsLibrary.Services.Interfaces;

public interface ITeamService
{
    Task<Team?> GetTeamById(int id);
    Task<Team?> GetTeamByName(string teamName);
    Task<bool> AddTeamAsync(Team team);
    Task<Team?> AddTeamAsync(TeamProfileDto teamProfile);
    Task<Team?> GenerateTeamFromParsedData(TeamProfileDto teamProfileDto);
    Task<bool> UpdateTeamAsync(TeamProfileDto teamProfile);
    Task<IEnumerable<Player>> GetPlayersOnTeamById(int teamId);
    Task<IEnumerable<Team>> GetAllTeamProfiles();
}