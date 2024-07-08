namespace CSProsLibrary.Models.Dtos.Viewmodels;

public class TeamProfile
{
    public IEnumerable<PlayerProfile> Players { get; set; }
    public string TeamName { get; set; }

    public static TeamProfile Convert(Team team)
    {
        return new TeamProfile()
        {
            TeamName = team.Name,
            Players = team.Players == null ? Enumerable.Empty<PlayerProfile>() : team.Players.Select(p => PlayerProfile.Convert(p))
        };
    }
}