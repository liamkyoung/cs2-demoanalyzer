using CSProsLibrary.Models.Enums;

namespace CSProsLibrary.Models;

public class Skin
{
    public int Id { get; set; }
    public int WeaponId { get; set; }
    public required Weapon Weapon { get; set; }
    public required string Name { get; set; }
    public required SkinRarity Rarity { get; set; }
    public string ImgSrc { get; set; } = "";
    public int Likes { get; set; } = 0;
    public int Dislikes { get; set; } = 0;
}