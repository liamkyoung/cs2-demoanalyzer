using System.Collections;
using CSProsLibrary.Models;
using CSProsLibrary.Models.Dtos;
using CSProsLibrary.Models.Dtos.Viewmodels;
using CSProsLibrary.Models.Enums;

namespace CSProsLibrary.Repositories.Interfaces;

public interface ISkinUsageRepository
{
    Task<IEnumerable<int>?> GetAllSkinIdsUsedInGameByPlayer(Player player, Game game);
    Task<IEnumerable<int>> GetAllSkinIdsUsedByPlayer(Player player);
    Task<Skin?> GetWeaponSkinUsedInGameByPlayer(Player player, Game game, Weapon weapon);
    Task AddSkinUsageAsync(Skin skin, Player player, Game game, int kills);
    Task UpdateSkinUsageAsync(Player player, Skin skin, Game game, int additionalKills);
    Task<bool> PlayerHasSkinUsage(Player player, Skin skin, Game game);
    Task<IEnumerable<SkinFrequencyDto>> GetSkinFrequenciesForPlayer(Player player);
    Task<int> GetSkinKillsForPlayer(Player player, int skinId);

    Task<PlayersUsingSkinDto> GetMostPopularPlayersUsingSkin(Skin skin, int limit = 5);

    Task<IEnumerable<SkinProfile>> GetMostUsedSkins(Weapon weapon, Player player, int limit = 3);

    Task<IEnumerable<Skin>> GetSkinPopularityForWeaponRanked(Weapon weapon, int limit = 5);
}