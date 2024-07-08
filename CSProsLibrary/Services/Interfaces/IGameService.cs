using CSProsLibrary.Models;
using CSProsLibrary.Models.Dtos;

namespace CSProsLibrary.Services.Interfaces;

public interface IGameService
{
    Task<bool> HasGameBeenProcessed(int id);
    Task<bool> AddGameAsync(Game game);

    Task<Game?> GenerateGameFromMatchData(MatchResultDto data, Team teamA, Team teamB);
    Task<bool> HasGameBeenProcessed(string hltvLink);
    Task<Game?> GetUnparsedGameByTeamsAndMap(int teamAId, int teamBId, int mapId); // Hard to find another way to match up a game.
    Task<Map?> GetMapByMapName(string mapName);
    Task<Map?> GetMapByDemoName(string mapName);
    Task SetGameAsParsed(int gameId);
}