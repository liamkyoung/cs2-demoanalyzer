namespace CSProsLibrary.Models.Dtos;

public class MatchResultDto
{
    public required int Id { get; set; }
    public required string TeamAName { get; set; }
    public required string TeamBName { get; set; }
    
    public required int TeamAScore { get; set; }
    public required int TeamBScore { get; set; }
    public required string TeamAProfileLink { get; set; }
    public required string TeamBProfileLink { get; set; }
    public required int NumberOfMaps { get; set; }
    public required string TournamentName { get; set; }
    public required int HltvStars { get; set; }
    public required string HltvLink { get; set; }
    public required string MapName { get; set; }
    public required DateTimeOffset StartedAt { get; set; }
}