using CSProsLibrary.Models.Enums;

namespace CSProsLibrary.Models.Dtos.Viewmodels;

public class WeaponProfile
{
    public string WeaponName { get; set; }
    public string NormalizedName { get; set; }
    public string ProfileImage { get; set; }
    public WeaponType WeaponType { get; set; }
    public TeamExclusivity TeamAssigned { get; set; }

    // public static WeaponProfile Convert(Weapon weapon)
    // {
    //     return new WeaponProfile()
    //     {
    //         WeaponName = weapon.Name,
    //         NormalizedName = weapon.NormalizedName,
    //         ProfileImage = weapon.
    //     }
    // }
}