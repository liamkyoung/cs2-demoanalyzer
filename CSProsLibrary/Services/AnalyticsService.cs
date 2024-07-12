using CSProsLibrary.Models.Dtos.Viewmodels;
using CSProsLibrary.Repositories.Interfaces;
using CSProsLibrary.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CSProsLibrary.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly ILogger<AnalyticsService> _logger;
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameRepository _gameRepository;
    private readonly ISkinRepository _skinRepository;

    public AnalyticsService(IPlayerRepository playerRepository, IGameRepository gameRepository,
        ISkinRepository skinRepository, ILogger<AnalyticsService> logger)
    {
        _playerRepository = playerRepository;
        _gameRepository = gameRepository;
        _skinRepository = skinRepository;
        _logger = logger;

    }
    
    public async Task<AppStatsDto> GetAppStats()
    {
        var games = await _gameRepository.GetNumberOfTotalGames();
        var kills = await _skinRepository.GetNumberOfKills();
        var skins = await _skinRepository.GetNumberOfSkins();
        var minutes = games * 34; // average found here https://tradeit.gg/blog/how-long-are-cs2-games/#:~:text=CS2%20matches%20can%20last%20anywhere,depending%20on%20the%20match%20type;

        return new AppStatsDto()
        {
            TotalGames = games,
            TotalKills = kills,
            TotalSkins = skins,
            TotalMinutes = minutes
        };
    }
}