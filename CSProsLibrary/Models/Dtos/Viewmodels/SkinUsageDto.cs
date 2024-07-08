namespace CSProsLibrary.Models.Dtos.Viewmodels;

public class SkinUsageDto
{
    public required PlayerProfile Player { get; init; }
    public int GamesUsed { get; init; } // How many games has a player played
    public int TotalKills { get; init; }
}