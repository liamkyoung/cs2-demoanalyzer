using System.ComponentModel.DataAnnotations.Schema;

namespace CSProsLibrary.Models;

public class SkinUsage
{
    public int PlayerId { get; set; }
    
    public int GameId { get; set; }
    
    public int SkinId { get; set; }
    
    public int KillsInGame { get; set; } = 0;
}