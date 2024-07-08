namespace CSProsLibrary.Models.Dtos;

public class TeamProfileDto
{
    public required string Name { get; init; }
    public required IEnumerable<string> PlayerHltvLinks { get; init; }
    public required string CountryName { get; init; }
    public required string HltvProfile { get; init; }
    public required int HltvRanking { get; init; }
    public required string? CoachName { get; init; }
    public required string? ImageSrc { get; init; }
}