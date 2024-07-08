using CSProsLibrary.Models.Enums;
using System;

namespace CSProsLibrary.Models.Dtos.Viewmodels;

public class SkinProfile
{
    public required string Uri { get; set; }
    public required string ImageSrc { get; set; }
    public required string WeaponName { get; set; }
    public required string SkinName { get; set; }
    public required SkinRarity SkinRarity { get; set; }
    public int? NumberOfKills { get; set; }
    public int? GamesUsed { get; set; }

    public static SkinProfile Convert(Skin skin, int? numberOfKills = null, int? gamesUsed = null)
    {
        var fSkinName = System.Uri.EscapeDataString(skin.Name);
        var fWeaponName = System.Uri.EscapeDataString(skin.Weapon.NormalizedName);
        
        return new SkinProfile()
        {
            Uri = $"/skins/{fWeaponName}/{fSkinName}",
            ImageSrc = skin.ImgSrc,
            SkinName = skin.Name,
            NumberOfKills = numberOfKills,
            GamesUsed = gamesUsed,
            SkinRarity = skin.Rarity,
            WeaponName = skin.Weapon.Name
        };
    }
}