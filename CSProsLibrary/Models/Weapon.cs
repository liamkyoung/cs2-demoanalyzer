using CSProsLibrary.Models.Enums;

namespace CSProsLibrary.Models;

public class Weapon
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string NormalizedName { get; set; } // used for urls on frontend
    public string DemoName { get; set; }
    public WeaponType Type { get; set; }
    public TeamExclusivity TeamAssigned { get; set; }
}