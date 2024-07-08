using CSProsLibrary.Models;

namespace CSProsLibrary.Repositories.Interfaces;

public interface IPlayerRepository
{
     Task<Player?> GetByIdAsync(int id);
     Task<Player?> GetByGamerTagAsync(string gamerTag);
     Task AddAsync(Player player);
     Task UpdateAsync(Player player);
     Task AddRangeAsync(IEnumerable<Player> players);
     Task<Player?> GetByHltvLinkAsync(string hltvLink);
     Task<IEnumerable<Player>> GetTrendingPlayers(TimeSpan timePeriod, int limit = 5);
     Task<IEnumerable<string>> GetAllPlayerGamerTags();
}