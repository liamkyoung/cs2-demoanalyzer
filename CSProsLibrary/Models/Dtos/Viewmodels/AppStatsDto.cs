namespace CSProsLibrary.Models.Dtos.Viewmodels;

public class AppStatsDto
{
    public required int TotalKills { get; init; }
    public required int TotalGames { get; init; }
    public required int TotalMinutes { get; init; }
    public required int TotalSkins { get; init; }
}