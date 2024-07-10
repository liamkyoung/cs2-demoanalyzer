namespace CSProsLibrary.Models.Dtos.Viewmodels;

public class PlayerProfile
{
    public required string PlayerName { get; set; }
    public required string Uri { get; set; }
    
    public string? TeamName { get; set; }
    public string? TeamProfileImage { get; set; }
    
    public string? CountryAbbreviation { get; set; }
    public string? ProfileImage { get; set; }

    public static PlayerProfile Convert(Player player)
    {
        var fGamerTag = System.Uri.EscapeDataString(player.GamerTag);
        
        return new PlayerProfile()
        {
            PlayerName = player.GamerTag,
            Uri = $"/players/{fGamerTag}",
            TeamProfileImage = player.Team?.ImageSrc,
            TeamName = player.Team?.Name,
            CountryAbbreviation = player.Country?.AbbreviatedName,
            ProfileImage = player.ProfileImage
        };
    }
}