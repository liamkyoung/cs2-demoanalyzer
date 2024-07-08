using CSProsLibrary.Models.Dtos.Viewmodels;

namespace CSProsLibrary.Models.Dtos;

public class PopularSkinListDto
{
    public Weapon Weapon { get; set; }
    public IEnumerable<SkinProfile> Skins { get; set; }
}