using CSProsLibrary.Models;
using CSProsLibrary.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CSProsLibrary.Repositories;

public class GameRepository : IGameRepository
{
    private readonly ApplicationDbContext _context;

    public GameRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Game?> GetGameByIdAsync(int id)
    {
        return await _context.Games.FindAsync(id); // Id will be found in the HLTV url
    }

    public IEnumerable<Game>? GetGamesByTeamsAndMap(int teamAId, int teamBId, int mapId)
    {
        try
        {
            return _context.Games.Where(g => 
                (g.TeamA.Id == teamAId && g.TeamB.Id == teamBId) ||
                (g.TeamA.Id == teamBId && g.TeamB.Id == teamAId) &&
                g.GameMap.Id == mapId);
        }
        catch (Exception e)
        { }

        return null;
    }

    public async Task SetGameAsParsed(int gameId)
    {
        var foundGame = await _context.Games.Where(g => g.Id == gameId).FirstOrDefaultAsync();
        
        if (foundGame != null)
        {
            foundGame.MatchParsed = true;
            await _context.SaveChangesAsync();
        }
    }
    
    public IEnumerable<Game>? GetGamesByHltvLinkAsync(string hltvLink)
    {
        try
        {
            return _context.Games.Where(g => g.HltvLink == hltvLink); // Id will be found in the HLTV url
        }
        catch (Exception e)
        { }

        return null;
    }
    

    public async Task AddGameAsync(Game game)
    {
        await _context.Games.AddAsync(game);
        await _context.SaveChangesAsync();
    }
}