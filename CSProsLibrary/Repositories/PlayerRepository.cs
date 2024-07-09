using System.Linq;
using CSProsLibrary.Models;
using CSProsLibrary.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CSProsLibrary.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly ApplicationDbContext _context;

    public PlayerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Player?> GetByIdAsync(int id)
    {
        return await _context.Players.Include(p => p.Country).FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public async Task<Player?> GetByGamerTagAsync(string gamerTag)
    {
        return await _context.Players
            .Where(player => player.GamerTag.ToLower() == gamerTag.ToLower())
            .Include(p => p.Country)
            .Include(p => p.Team)
            .FirstOrDefaultAsync();
    }
    
    public async Task<Player?> GetByHltvLinkAsync(string hltvLink)
    {
        return await _context.Players.Where(player => player.HltvLink == hltvLink).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Player>> GetTrendingPlayers(TimeSpan timePeriod, int limit = 4)
    {
        // Get players with the most kills in the given timespan of games.
        var players = await _context.Games.Where(g => (g.StartedAt + timePeriod) > DateTimeOffset.UtcNow)
            .Join(_context.SkinUsages, game => game.Id, usage => usage.GameId, (game, usage) => usage)
            .GroupBy(u => u.PlayerId)
            .OrderByDescending(group => group.Sum(i => i.KillsInGame))
            .Select(group => group.Key)
            .Join(_context.Players, playerId => playerId, player => player.Id, (id, player) => player)
            .Include(p => p.Country)
            .Include(p => p.Team)
            .Take(limit)
            .ToListAsync();

        return players;
    }

    public async Task AddAsync(Player player)
    {
        _context.Players.Add(player);
        await _context.SaveChangesAsync();
    }
    
    public async Task AddRangeAsync(IEnumerable<Player> players) // Causing errors?
    {
        _context.Players.AddRange(players);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Player player)
    {
        var foundPlayer = await GetByHltvLinkAsync(player.HltvLink);

        if (foundPlayer != null)
        {
            foundPlayer = player;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<string>> GetAllPlayerGamerTags()
    {
        return await _context.Players.Select(p => p.GamerTag).ToListAsync();
    }
}