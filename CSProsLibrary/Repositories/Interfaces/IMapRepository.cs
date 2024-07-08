using CSProsLibrary.Models;

namespace CSProsLibrary.Repositories.Interfaces;

public interface IMapRepository
{
    Task<Map?> GetMapByNameAsync(string mapName);
    Task<Map?> GetMapByDemoName(string mapName);
    Task IncrementGamesPlayedOnMap(int mapId);
}