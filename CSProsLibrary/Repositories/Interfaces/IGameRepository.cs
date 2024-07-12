using CSProsLibrary.Models;
namespace CSProsLibrary.Repositories.Interfaces;

public interface IGameRepository
{
    Task<Game?> GetGameByIdAsync(int id); // Need to change to support BO3's
    Task AddGameAsync(Game game);
    Task SetGameAsParsed(int gameId);
    IEnumerable<Game>? GetGamesByHltvLinkAsync(string hltvLink);
    IEnumerable<Game>? GetGamesByTeamsAndMap(int teamAId, int teamBId, int mapId);
    Task<int> GetNumberOfTotalGames();
}