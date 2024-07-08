using HtmlAgilityPack;
using CSProsLibrary.Models;
using CSProsLibrary.Models.Dtos;
using CSProsLibrary.Models.Dtos.Viewmodels;
using CSProsLibrary.Models.Enums;
using CSProsLibrary.Repositories.Interfaces;
using CSProsLibrary.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CSProsLibrary.Services;

public class SkinService : ISkinService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly ISkinUsageRepository _skinUsageRepository;
    private readonly ISkinRepository _skinRepository;
    private readonly IWeaponRepository _weaponRepository;
    private readonly ILogger<SkinService> _logger;

    public SkinService(IPlayerRepository playerRepository, ISkinRepository skinRepository, ISkinUsageRepository skinUsageRepository, IWeaponRepository weaponRepository, ILogger<SkinService> logger)
    {
        _playerRepository = playerRepository;
        _skinUsageRepository = skinUsageRepository;
        _skinRepository = skinRepository;
        _weaponRepository = weaponRepository;
        _logger = logger;
    }

    public ParsedSkinInfoDto? GetSkinInfoFromWeaponItemId(long weaponItemId)
    {
        var url = $"https://csgo.exchange/item/{weaponItemId}";
        HtmlWeb web = new HtmlWeb();

        var htmlDoc = web.Load(url);

        try
        {
            var fullSkinText = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='detailItem']/div[2]/h3").InnerText;
            var skinRarityText = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='detailItem']/div[2]/div")
                .InnerText;
            var imgContainer = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='detailItem']/div[1]/div");

            var imgSrcStyleText = imgContainer?.GetAttributeValue("style", string.Empty);

            if (fullSkinText == null || skinRarityText == null || imgSrcStyleText == null)
            {
                return null;
            }

            var skinName = ParseSkinName(fullSkinText);
            var imgSrc = ParseImgSrc(imgSrcStyleText);
            var skinRarity = ParseSkinRarity(skinRarityText);
            
            if (skinName != null && imgSrc != null && skinRarity != null)
            {
                return new ParsedSkinInfoDto()
                {
                    SkinName = skinName,
                    ImgSrc = imgSrc,
                    SkinRarity = skinRarity.Value
                };
            }
        }
        catch (Exception e)
        {
            // _logger.LogError($"Error parsing page on csgo.exchange/items/{weaponItemId}", e.Message);
        }

        _logger.LogInformation($"Could not find skin: https://csgo.exchange/item/{weaponItemId}");
        return null;


        string? ParseSkinName(string? skinName)
        {
            if (skinName == null) return null;
            
            // Formatting Results
            // OG Format:
            // Name:   StatTrak AK-47 | Redline -> Redline
            var splitWeaponName = skinName.Split('|', StringSplitOptions.TrimEntries);

            if (splitWeaponName.Length == 2)
            {
                return splitWeaponName[1];
            }

            return null;
        }

        string? ParseImgSrc(string? imgSrcStyleText)
        {
            if (imgSrcStyleText == null) return null; 
            // ImgSrc: width:180px; height:180px;background-image:url(URL) -> URL
            var splitStrings = imgSrcStyleText.Split('(', StringSplitOptions.TrimEntries);

            if (splitStrings.Length == 2)
            {
                return splitStrings[1].TrimEnd(')');
            }

            return null;
        }

        SkinRarity? ParseSkinRarity(string skinRarityText)
        {
            // Cannot do Enum.TryParse() because of "Grade" after consumer, industrial, etc. and not covert, extraordinary, etc.
            return skinRarityText.ToLower() switch 
            {
                "consumer grade" => SkinRarity.Consumer,
                "industrial grade" => SkinRarity.Industrial,
                "mil-spec grade" => SkinRarity.MilSpec,
                "restricted" => SkinRarity.Restricted,
                "classified" => SkinRarity.Classified,
                "covert" => SkinRarity.Covert,
                "extraordinary" => SkinRarity.Extraordinary,
                "contraband" => SkinRarity.Contraband,
                _ => null
            };
        }
    }

    public async Task<Skin?> GetSkinByWeaponItemIdAsync(long weaponItemId)
    {
        return await _skinRepository.GetSkinByWeaponItemId(weaponItemId);
    }

    public async Task<Skin?> GetSkinByNameAndWeaponId(string skinName, Weapon weapon)
    {
        return await _skinRepository.GetSkinByNameAndWeapon(skinName, weapon);
    }

    public async Task AddWeaponItemAsync(long weaponItemId, Skin? skin)
    {
        await _skinRepository.AddWeaponItemAsync(weaponItemId, skin);
    }

    public async Task<bool> HasWeaponItem(long weaponItemId)
    {
        return await _skinRepository.HasWeaponItem(weaponItemId);
    }

    public Skin GenerateSkinFromData(ParsedSkinInfoDto skinInfo, Weapon weapon)
    {
        return new Skin()
        {
            Name = skinInfo.SkinName,
            ImgSrc = skinInfo.ImgSrc,
            Rarity = skinInfo.SkinRarity,
            Weapon = weapon,
            WeaponId = weapon.Id,
        };
    }

    public async Task<IEnumerable<PopularSkinListDto>> GetPopularSkinsForAllWeapons()
    {
        var weaponList = await _weaponRepository.GetAllWeapons();

        var popularSkinsList = new List<PopularSkinListDto>();
        foreach (var weapon in weaponList)
        {
            var weaponSkins = await _skinUsageRepository.GetSkinPopularityForWeaponRanked(weapon);
            
            popularSkinsList.Add(new PopularSkinListDto { Weapon = weapon, Skins = weaponSkins.Select(skin => SkinProfile.Convert(skin)) });
        }

        return popularSkinsList;
    }

    public async Task<IEnumerable<Skin>> GetPopularSkinsForWeapon(Weapon weapon)
    {
        var skinsRanked = await _skinUsageRepository.GetSkinPopularityForWeaponRanked(weapon, 4);

        return skinsRanked;
    }

    public async Task<IEnumerable<SkinProfile>> GetMostUsedSkins(Weapon weapon, Player player)
    {
        return await _skinUsageRepository.GetMostUsedSkins(weapon, player);
    }

    public async Task<IEnumerable<Skin>> GetAllSkinsUsedByPlayer(Player player)
    {
        var skinIds = await _skinUsageRepository.GetAllSkinIdsUsedByPlayer(player);

        var skins = new List<Skin>();
        foreach (var skinId in skinIds)
        {
            var skin = await _skinRepository.GetSkinByIdAsync(skinId);

            if (skin != null)
            {
                skins.Add(skin);
            }
        }

        return skins;
    }

    public async Task<IEnumerable<Skin>?> GetAllSkinsUsedInGameByPlayer(Player player, Game game)
    {
        var skinIds = await _skinUsageRepository.GetAllSkinIdsUsedInGameByPlayer(player, game);

        if (skinIds == null)
        {
            return Enumerable.Empty<Skin>();
        }
        
        var listOfSkins = new List<Skin>();

        foreach (var skinId in skinIds)
        {
            var skin = await _skinRepository.GetSkinByIdAsync(skinId);

            if (skin != null)
            {
                listOfSkins.Add(skin);
            }
        }

        return listOfSkins;
    }
    

    public async Task AddOrUpdateSkinUsageAsync(Skin skin, Player player, Game game, int kills)
    {
        if (!await _skinUsageRepository.PlayerHasSkinUsage(player, skin, game))
        {
            await _skinUsageRepository.AddSkinUsageAsync(skin, player, game, kills);
            return;
        }
        
        // Rare case where 2 occurrences of the same skin (with different weaponItemId's) occur in a single game, need to combine the kills for each weapon.
        await _skinUsageRepository.UpdateSkinUsageAsync(player, skin, game, kills); 
    }

    public async Task<Skin?> GetSkinByIdAsync(int id)
    {
        return await _skinRepository.GetSkinByIdAsync(id);
    }

    public async Task AddSkinAsync(Skin skin)
    {
        await _skinRepository.AddSkinAsync(skin);
    }

    public async Task<IEnumerable<Skin>> GetAllSkinsForWeapon(Weapon weapon)
    {
        return await _skinRepository.GetAllSkinsForWeapon(weapon.Id);
    }

    public async Task<IEnumerable<Skin>> GetAllSkinsByRarity(SkinRarity rarity)
    {
        return await _skinRepository.GetAllSkinsByRarity(rarity);
    }

    public async Task<int> GetSkinKillsForPlayer(Player player, int skinId)
    {
        return await _skinUsageRepository.GetSkinKillsForPlayer(player, skinId);
    }

    public async Task<PlayersUsingSkinDto> GetMostPopularPlayersUsingSkin(Skin skin, int limit = 5)
    {
        return await _skinUsageRepository.GetMostPopularPlayersUsingSkin(skin, limit);
    }

    public async Task<IEnumerable<Skin>> GetTrendingSkins(TimeSpan timePeriod, int limit = 4)
    {
        return await _skinRepository.GetTrendingSkins(timePeriod, limit);
    }
}