using CSProsLibrary.Models;
using CSProsLibrary.Models.Dtos;
using CSProsLibrary.Models.Dtos.Viewmodels;
using CSProsLibrary.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CSProsLibrary.Repositories;

public class SkinUsageRepository : ISkinUsageRepository
{
    private readonly ApplicationDbContext _context;

    public SkinUsageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SkinProfile>> GetMostUsedSkins(Weapon weapon, Player player, int limit = 3)
    {
        return await _context.Skins.Where(skin => skin.WeaponId == weapon.Id)
            .Join(_context.SkinUsages,
                skin => skin.Id,
                usage => usage.SkinId,
                (skin, usage) => new { skin, usage })
            .Where(group => group.usage.PlayerId == player.Id)
            .GroupBy(s => s.skin)
            .OrderByDescending(group => group.Sum(i => i.usage.KillsInGame))
            .Select(group => SkinProfile.Convert(group.Key, group.Sum(i => i.usage.KillsInGame), group.Count()))
            .Take(limit)
            .ToListAsync();
    }
    

    public async Task<IEnumerable<int>> GetAllSkinIdsUsedByPlayer(Player player)
    {
        var skinUsages = await _context.SkinUsages.Where(skinUsage => skinUsage.PlayerId == player.Id).ToListAsync();

        return skinUsages.Select(s => s.SkinId);
    }
    
    public async Task<IEnumerable<int>?> GetAllSkinIdsUsedInGameByPlayer(Player player, Game game)
    {
        var query = _context.SkinUsages.Where(skinUsage => skinUsage.PlayerId == player.Id && skinUsage.GameId == game.Id);

        return await query.Select(skinUsage => skinUsage.SkinId).ToListAsync();
    }

    public async Task<Skin?> GetWeaponSkinUsedInGameByPlayer(Player player, Game game, Weapon weapon)
    {
        var allSkinsUsedInGame = await GetAllSkinIdsUsedInGameByPlayer(player, game);

        if (allSkinsUsedInGame != null)
        {
            var matchingSkin = _context.Skins
                .FirstOrDefaultAsync(skin => skin.Weapon.Id == weapon.Id && allSkinsUsedInGame.Any(skinId => skinId == skin.Id));

            return await matchingSkin;
        }

        return null;
    }

    public async Task AddSkinUsageAsync(Skin skin, Player player, Game game, int kills = 0)
    {
        var skinUsage = new SkinUsage()
        {
            GameId = game.Id, 
            PlayerId = player.Id, 
            SkinId = skin.Id,
            KillsInGame = kills
        };
        
        _context.SkinUsages.Add(skinUsage);
        await _context.SaveChangesAsync();
    }

    private async Task<SkinUsage?> GetSkinUsage(Player player, Skin skin, Game game)
    {
        var skinUsage = await _context.SkinUsages.FirstOrDefaultAsync(skinUsage =>
            (player.Id == skinUsage.PlayerId && skinUsage.SkinId == skin.Id && skinUsage.GameId == game.Id));

        return skinUsage;
    }

    public async Task<bool> PlayerHasSkinUsage(Player player, Skin skin, Game game)
    {
        return await GetSkinUsage(player, skin, game) != null;
    }

    public async Task UpdateSkinUsageAsync(Player player, Skin skin, Game game, int additionalKills)
    {
        var skinUsage = await GetSkinUsage(player, skin, game);

        if (skinUsage != null)
        {
            skinUsage.KillsInGame += additionalKills;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Skin>> GetSkinPopularityForWeaponRanked(Weapon weapon, int limit = 5)
    {
        // GET SKIN IDS
        // JOIN WITH SKINUSAGE
        // COUNT KILLS
        // GET TOP 
        var topSkins = _context.Skins.Where(s => s.WeaponId == weapon.Id)
            .Join(_context.SkinUsages,
                skin => skin.Id,
                usage => usage.SkinId,
                (skin, usage) => new { skin, usage })
            .GroupBy(group => group.skin)
            .OrderByDescending(group => group.Sum(i => i.usage.KillsInGame))
            .Take(limit)
            .Select(s => s.Key);

        return await topSkins.ToListAsync();
    }

    public async Task<IEnumerable<SkinFrequencyDto>> GetSkinFrequenciesForPlayer(Player player)
    {
        var playerSkins = await _context.SkinUsages
            .Where(skinUsage => skinUsage.PlayerId == player.Id)
            .Join(_context.Skins, // The table you're joining with
                skinUsage => skinUsage.SkinId, // Key from the source table
                skin => skin.Id, // Key from the table you're joining
                (skinUsage, skin) => new { skinUsage, skin }) // Result selector
            .GroupBy(sg => sg.skin) // Group by the Skin object
            .Select(group => new SkinFrequencyDto
            {
                PlayerId = player.Id,
                SkinId = group.Key.Id,
                WeaponId = group.Key.WeaponId,
                GamesUsed = group.Count(), // Frequency of each skin usage
                TotalKills = group.Sum(s => s.skinUsage.KillsInGame)
            })
            .OrderByDescending(dto => dto.GamesUsed) // Order by frequency descending
            .ToListAsync();

        return playerSkins;
    }

    // Will show with most use
    public async Task<PlayersUsingSkinDto> GetMostPopularPlayersUsingSkin(Skin skin, int limit = 5)
    {
        var skinUsages = await _context.SkinUsages
            .Where(usage => usage.SkinId == skin.Id)
            .ToListAsync();

        var groupedUsages = skinUsages
            .GroupBy(usage => usage.PlayerId)
            .Select(group => new 
            {
                PlayerId = group.Key,
                GamesUsed = group.Count(),
                TotalKills = group.Sum(usage => usage.KillsInGame)
            })
            .ToList();

        var highestUsePlayers = groupedUsages
            .Join(_context.Players,
                usages => usages.PlayerId,
                player => player.Id,
                (usages, player) => new SkinUsageDto
                {
                    GamesUsed = usages.GamesUsed,
                    TotalKills = usages.TotalKills,
                    Player = PlayerProfile.Convert(player)
                })
            .OrderByDescending(x => x.GamesUsed)
            .ThenByDescending(x => x.TotalKills)
            .Take(limit)
            .ToList();

        return new PlayersUsingSkinDto()
        {
            Skin = SkinProfile.Convert(skin),
            Players = highestUsePlayers
        };
    }

    public async Task<int> GetSkinKillsForPlayer(Player player, int skinId)
    {
        var kills = await _context.SkinUsages
            .Where(skinUsage => skinUsage.PlayerId == player.Id && skinUsage.SkinId == skinId)
            .SumAsync(group => group.KillsInGame);

        return kills;
    }
}