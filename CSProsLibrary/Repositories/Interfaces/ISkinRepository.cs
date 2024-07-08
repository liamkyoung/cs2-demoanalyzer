using CSProsLibrary.Models;
using CSProsLibrary.Models.Enums;

namespace CSProsLibrary.Repositories.Interfaces;

public interface ISkinRepository
{
    // Skins
    Task<Skin?> GetSkinByIdAsync(int id);
    Task AddSkinAsync(Skin skin);
    Task<IEnumerable<Skin>> GetAllSkinsForWeapon(int weaponId);
    Task<IEnumerable<Skin>> GetAllSkinsByRarity(SkinRarity rarity);
    Task<Skin?> GetSkinByWeaponItemId(long weaponItemId);
    Task<Skin?> GetSkinByNameAndWeapon(string skinName, Weapon weapon);
    Task AddWeaponItemAsync(long weaponItemId, Skin? skin);
    Task<bool> HasWeaponItem(long weaponItemId);
    Task<IEnumerable<Skin>> GetTrendingSkins(TimeSpan timePeriod, int limit = 4);
}