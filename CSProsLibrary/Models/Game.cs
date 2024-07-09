using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSProsLibrary.Models;

public class Game
{
    public int Id { get; set; }
    
    public required Team TeamA { get; set; }
    public required Team TeamB { get; set; }
    
    public required int TeamAScore { get; set; }
    public required int TeamBScore { get; set; }
    public required Team Winner { get; set; }
    public int HltvStars { get; set; }
    public string HltvLink { get; set; }
    public int GameMapId { get; set; }
    public required Map GameMap { get; set; }
    
    public required DateTimeOffset StartedAt { get; set; }
    public ICollection<SkinUsage> SkinUsages { get; set; }
    public bool MatchParsed { get; set; } = false;
    public bool MatchHadParsingError { get; set; } = false;
}