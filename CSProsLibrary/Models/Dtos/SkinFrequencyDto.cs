using CSProsLibrary.Models.Dtos.Viewmodels;

namespace CSProsLibrary.Models.Dtos;

public class SkinFrequencyDto
{
    public required int PlayerId { get; init; }
    public required int SkinId { get; init; }
    public required int WeaponId { get; init; }
    public required int GamesUsed { get; init; }
    public required int TotalKills { get; init; }
}