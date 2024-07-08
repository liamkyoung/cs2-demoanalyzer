using System.Security.Cryptography;
using System.Text;
using CSProsLibrary.Repositories.Interfaces;
using CSProsLibrary.Services.Interfaces;
using CSProsLibrary.Models;
using CSProsLibrary.Models.Dtos;
using Microsoft.Extensions.Logging;

namespace CSProsLibrary.Services;

public class GameService : IGameService
{
    private readonly ILogger<GameService> _logger;
    private readonly IGameRepository _gameRepository;
    private readonly IMapRepository _mapRepository;
    private readonly ITeamService _teamService;
    
    public GameService(IGameRepository gameRepository, IMapRepository mapRepository, ITeamService teamService, ILogger<GameService> logger)
    {
        _logger = logger;
        _mapRepository = mapRepository;
        _gameRepository = gameRepository;
        _teamService = teamService;
    }
    
    public async Task<Map?> GetMapByMapName(string mapName)
    {
        return await _mapRepository.GetMapByNameAsync(mapName);
    }
    
    public async Task<Map?> GetMapByDemoName(string mapName)
    {
        return await _mapRepository.GetMapByDemoName(mapName);
    }

    public async Task SetGameAsParsed(int gameId)
    { 
        var game = await _gameRepository.GetGameByIdAsync(gameId);

        if (game != null)
        {
            await _gameRepository.SetGameAsParsed(gameId);
            await _mapRepository.IncrementGamesPlayedOnMap(game.GameMap.Id);
        }
        
    }

    public async Task<bool> HasGameBeenProcessed(string hltvLink)
    {
        var games = _gameRepository.GetGamesByHltvLinkAsync(hltvLink);
        
        return games != null && games.Any(); // Checks if there are any games with Hltv Link already existing in DB
    }
    
    public async Task<bool> HasGameBeenProcessed(int id)
    {
        var game = await _gameRepository.GetGameByIdAsync(id); // Id needs to be the 2370568 in https://www.hltv.org/matches/2370568/mibr-v...
        
        return game != null;
    }

    public async Task<Game?> GetUnparsedGameByTeamsAndMap(int teamAId, int teamBId, int mapId)
    {
        var potentialGames = _gameRepository.GetGamesByTeamsAndMap(teamAId, teamBId, mapId);

        if (potentialGames != null)
        {
            return potentialGames.FirstOrDefault(g => !g.MatchParsed);
        }

        return null;
    }
    

    public async Task<bool> AddGameAsync(Game game)
    {
        try
        {
            await _gameRepository.AddGameAsync(game);
            _logger.LogInformation($"Added game to DB: {game.HltvLink} - {game.GameMap.Name}");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError($"Could not add game to DB: {game.HltvLink} | {e.Message}");
        }

        return false;
    }
    
    public async Task<Game?> GenerateGameFromMatchData(MatchResultDto data, Team teamA, Team teamB)
    {
        try
        {
            var winner = data.TeamAScore > data.TeamBScore ? teamA : teamB;
            var map = await _mapRepository.GetMapByNameAsync(data.MapName.ToLower());
            
            if (map == null)
            {
                return null;
            }
            
            // Todo: GameId
            return new Game()
            {
                TeamA = teamA,
                TeamB = teamB,
                GameMap = map,
                TeamAScore = data.TeamAScore,
                TeamBScore = data.TeamBScore,
                HltvLink = data.HltvLink,
                Winner = winner,
                StartedAt = data.StartedAt
            };
            
        }
        catch (Exception e)
        {
            _logger.LogError($"Could not create game from match data | {e.StackTrace}");
        }

        return null;
    }
}  