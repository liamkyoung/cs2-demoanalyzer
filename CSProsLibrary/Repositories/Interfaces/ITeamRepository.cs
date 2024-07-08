using CSProsLibrary.Models;

namespace CSProsLibrary.Repositories.Interfaces;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(int id);
    Task<Team?> GetByNameAsync(string teamName);
    Task<IEnumerable<Team>?> GetTeamsByCountryAsync(Country country);
    Task<IEnumerable<Team>?> GetTeamsByHLTVRanking(int minimumRanking, int maximumRanking);
    Task<bool> AddAsync(Team team);
    Task<bool> UpdateAsync(Team team);

    Task<IEnumerable<Player>> GetPlayersOnTeamById(int teamId);
    Task<IEnumerable<Team>> GetAllTeamProfiles();
}