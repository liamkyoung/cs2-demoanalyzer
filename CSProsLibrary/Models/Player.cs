using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CSProsLibrary.Models;

public class Player
{
    public int Id { get; set; }
    
    [MaxLength(75)]
    public string Name { get; set; }
    
    [MaxLength(50)]
    public string GamerTag { get; set; }

    public string? ProfileImage { get; set; } = null;
    
    // Define the CountryId foreign key property
    public int? CountryId { get; set; }
    public virtual Country? Country { get; set; } // Country can be null for lesser known players
    
    public int Likes { get; set; } = 0;
    public int Dislikes { get; set; } = 0;
    public string[]? Nicknames { get; set; } = null;
    
    [JsonIgnore]
    public virtual Team? Team { get; set; }
    public int? TeamId { get; set; } // Team foreign key

    public PlayerSocials? Socials { get; set; } = null;
    public int Age { get; set; }
    
    [MaxLength(100)]
    public string HltvLink { get; set; }
    public DateTimeOffset TimeSinceLastUpdated { get; set; } = DateTimeOffset.UtcNow; // Should value every week or so
}