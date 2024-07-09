using CSProsLibrary.Models;
using CSProsLibrary.Models.Dtos;

namespace CSProsLibrary.Services.Interfaces;

public interface IPlayerService
{
    Task<Player?> GetPlayerByGamerTag(string gamerTag);
    Task<Dictionary<Weapon, Skin?>> GetPlayerSkinsForGame(Player player, Game game);
    Task<IEnumerable<Player>> GetTeammates(Player player);
    Task<Dictionary<Weapon, Skin?>> GetMostUsedSkinsForPlayer(Player player);
    Task<IEnumerable<SkinFrequencyDto>?> GetPlayersMostUsedSkinsForWeapon(Player player, Weapon weapon);
    Task<Player?> GetPlayerByHltvLink(string hltvLink);
    Task<Player> GeneratePlayer(PlayerProfileDto playerProfileDto);
    Task<bool> AddPlayer(Player player);
    Task<bool> AddPlayer(PlayerProfileDto playerProfile);
    Task<bool> UpdatePlayer(PlayerProfileDto playerProfile);
    Task<bool> UpdatePlayer(Player player);
    Task<IEnumerable<Player>> GetTrendingPlayers(TimeSpan timePeriod, int limit);
    Task<Player?> GetPlayerMostSimilarToInGameName(string inGameName);

}