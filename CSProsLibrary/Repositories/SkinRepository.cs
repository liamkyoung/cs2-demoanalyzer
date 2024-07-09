using System.Linq;
using CSProsLibrary.Models;
using CSProsLibrary.Models.Enums;
using CSProsLibrary.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CSProsLibrary.Repositories;

public class SkinRepository : ISkinRepository
{
    private readonly ApplicationDbContext _context;

    public SkinRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Skin?> GetSkinByIdAsync(int id)
    {
        return await _context.Skins.FindAsync(id);
    }

    public async Task<Skin?> GetSkinByWeaponItemId(long weaponItemId)
    {
        var weaponItem = await _context.WeaponItems.FindAsync(weaponItemId);

        return weaponItem?.Skin;
    }

    public async Task<Skin?> GetSkinByNameAndWeapon(string skinName, Weapon weapon)
    {
        return await _context.Skins.Where(s => s.Name == skinName && s.WeaponId == weapon.Id).FirstOrDefaultAsync();
    }

    public async Task AddWeaponItemAsync(long weaponItemId, Skin? skin)
    {
        var weaponItem = new WeaponItem()
        {
            Id = weaponItemId,
            Skin = skin
        };

        await _context.WeaponItems.AddAsync(weaponItem);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasWeaponItem(long weaponItemId)
    {
        return await _context.WeaponItems.FirstOrDefaultAsync(item => item.Id == weaponItemId) != null;
    }

    public async Task AddSkinAsync(Skin skin)
    {
        _context.Skins.Add(skin);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Skin>> GetAllSkinsForWeapon(int weaponId)
    {
        return await _context.Skins.Where(skin => skin.WeaponId == weaponId).ToListAsync();
    }

    public async Task<IEnumerable<Skin>> GetAllSkinsByRarity(SkinRarity rarity)
    {
        return await _context.Skins.Where(skin => skin.Rarity == rarity).ToListAsync();
    }

    public async Task<IEnumerable<Skin>> GetTrendingSkins(TimeSpan timePeriod, int limit = 4)
    {
        var skins = await _context.Games.Where(g => (g.StartedAt + timePeriod) > DateTimeOffset.UtcNow)
            .Join(_context.SkinUsages, game => game.Id, usage => usage.GameId, (game, usage) => usage)
            .GroupBy(u => u.SkinId)
            .OrderByDescending(group => group.Sum(i => i.KillsInGame))
            .Select(group => group.Key)
            .Join(_context.Skins, skinId => skinId, skin => skin.Id, (id, skin) => skin)
            .Include(s => s.Weapon)
            .Take(limit)
            .ToListAsync();
        
        return skins;
    }
}