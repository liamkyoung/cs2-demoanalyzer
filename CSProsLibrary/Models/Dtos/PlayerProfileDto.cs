namespace CSProsLibrary.Models.Dtos;

public class PlayerProfileDto
{
    public required string RealName { get; set; }
    public required string GamerTag { get; set; }
    public required string HltvLink { get; set; }
    public required string? TeamName { get; set; }
    public required string? TwitterProfile { get; set; }
    public required string? TwitchProfile { get; set; }
    public required string? InstagramProfile { get; set; }
    public required string CountryName { get; set; }
    public required int Age { get; set; }
    public required string ProfileImage { get; set; }
    
}