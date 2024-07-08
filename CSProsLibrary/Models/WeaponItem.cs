using System.ComponentModel.DataAnnotations.Schema;

namespace CSProsLibrary.Models;

// Entity is used to distinguish specific game items between each players.
// Caching this is not explicitly necessary but helps to reduce the number of requests to skin apis.
public class WeaponItem
{
    public long Id { get; set; } // Corresponds to e.weaponItemId in Demo
    
    public Skin? Skin { get; set; } // Skin in csgo.exchange won't be found if inventory is private. Should still log to reduce lookups.
}