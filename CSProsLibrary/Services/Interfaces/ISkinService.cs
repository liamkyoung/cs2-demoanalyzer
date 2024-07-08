using CSProsLibrary.Models;
using CSProsLibrary.Models.Dtos;
using CSProsLibrary.Models.Dtos.Viewmodels;
using CSProsLibrary.Models.Enums;

namespace CSProsLibrary.Services.Interfaces;

public interface ISkinService
{
    ParsedSkinInfoDto? GetSkinInfoFromWeaponItemId(long weaponItemId);
    Task<IEnumerable<Skin>> GetAllSkinsUsedByPlayer(Player player);
    Task<IEnumerable<Skin>?> GetAllSkinsUsedInGameByPlayer(Player player, Game game);

    Task<IEnumerable<PopularSkinListDto>> GetPopularSkinsForAllWeapons();
    Task<IEnumerable<SkinProfile>> GetMostUsedSkins(Weapon weapon, Player player);
    
    Task AddOrUpdateSkinUsageAsync(Skin skin, Player player, Game game, int kills);
    
    Task<Skin?> GetSkinByIdAsync(int id);
    Task<Skin?> GetSkinByWeaponItemIdAsync(long weaponItemId);
    Task<bool> HasWeaponItem(long weaponItemId);
    Task<Skin?> GetSkinByNameAndWeaponId(string skinName, Weapon weapon);
    Task AddWeaponItemAsync(long weaponItemId, Skin? skin);
    Skin GenerateSkinFromData(ParsedSkinInfoDto skinInfo, Weapon weapon);
    
    Task AddSkinAsync(Skin skin);
    
    Task<IEnumerable<Skin>> GetAllSkinsForWeapon(Weapon weapon);
    Task<IEnumerable<Skin>> GetAllSkinsByRarity(SkinRarity rarity);
    
    Task<int> GetSkinKillsForPlayer(Player player, int skinId); // Total kills using a skin for a player

    Task<PlayersUsingSkinDto> GetMostPopularPlayersUsingSkin(Skin skin, int limit = 5); // Players with the most use of a give skin
    Task<IEnumerable<Skin>> GetPopularSkinsForWeapon(Weapon weapon);
    Task<IEnumerable<Skin>> GetTrendingSkins(TimeSpan timePeriod, int limit = 4);
}