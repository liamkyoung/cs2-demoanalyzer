using CSProsLibrary.Models;
using CSProsLibrary.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CSProsLibrary.Repositories;

public class MapRepository : IMapRepository
{
    private readonly IApplicationDbContext _context;

    public MapRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Map?> GetMapByNameAsync(string mapName)
    {
        return await _context.Maps.Where(p => p.Name.ToLower() == mapName).FirstOrDefaultAsync();
    }
    
    public async Task<Map?> GetMapByDemoName(string mapName)
    {
        return await _context.Maps.Where(p => p.DemoName.ToLower() == mapName).FirstOrDefaultAsync();
    }

    public async Task IncrementGamesPlayedOnMap(int mapId)
    {
        var map = await _context.Maps.Where(map => map.Id == mapId).FirstOrDefaultAsync();

        if (map != null)
        {
            map.GamesPlayedOnMap++;
            await _context.SaveChangesAsync();
        }
    }
}