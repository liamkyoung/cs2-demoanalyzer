using System.Text.Json.Serialization;

namespace CSProsLibrary.Models;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? ImageSrc { get; set; }
    
    public IEnumerable<Player>? Players { get; set; }
    public Country? Country { get; set; }
    public string HltvProfile { get; set; }
    public int HltvRanking { get; set; }
    public string? CoachName { get; set; }
    public DateTimeOffset TimeSinceLastUpdated { get; set; } = DateTimeOffset.UtcNow; // Should be updated every week or when users do it
}