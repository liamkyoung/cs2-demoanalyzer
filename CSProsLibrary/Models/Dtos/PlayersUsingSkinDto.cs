using CSProsLibrary.Models.Dtos.Viewmodels;

namespace CSProsLibrary.Models.Dtos;

public class PlayersUsingSkinDto
{
    public required IEnumerable<SkinUsageDto> Players { get; set; }
    public required SkinProfile Skin { get; set; }
}