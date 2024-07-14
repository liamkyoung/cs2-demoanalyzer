using CSProsLibrary.Models.Dtos;
using CSProsLibrary.Models.Enums;
using CSProsLibrary.Services.Interfaces.Scraping;
using CSProsLibrary.Services.Scraping.Pages;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace CSProsLibrary.Services.Scraping;

public class ScrapingService : IScrapingService
{
    private readonly ILogger<ScrapingService> _logger;
    private readonly PlayerPage _playerPage;
    private readonly TeamPage _teamPage;
    private readonly MatchPage _matchPage;
    private readonly ResultsListPage _resultsPage;
    private readonly IWebDriver _driver;

    public ScrapingService(IWebDriver driver, PlayerPage playerPage, TeamPage teamPage, MatchPage matchPage, ResultsListPage resultsPage, ILogger<ScrapingService> logger)
    {
        _driver = driver;
        _playerPage = playerPage;
        _teamPage = teamPage;
        _matchPage = matchPage;
        _resultsPage = resultsPage;
        _logger = logger;
    }
    
    public async Task<PlayerProfileDto?> ParsePlayerProfile(string playerProfileHref)
    {
        _playerPage.GoToPage(playerProfileHref);
        return _playerPage.GetPlayerData();
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
    
    public TeamProfileDto? ParseTeamPage(string teamUrl)
    {
        _logger.LogInformation($"Parsing team: {teamUrl}");
        _teamPage.GoToPage(teamUrl);
        
        var teamInfo = _teamPage.GetTeamInfo();

        if (teamInfo == null)
        {
            _logger.LogError($"Could not parse team data from page. {teamUrl}");
            return null;
        }

        _logger.LogInformation($"Parsed Team: {teamInfo.Name}");
        return teamInfo;
    }

    public HashSet<string> GetMatchLinks()
    {
        return _resultsPage.GetMatches();
    }

    public void GoToPage(string href)
    {
        _driver.Navigate().GoToUrl(href);
    }

    public void GoToMatchPage(string href)
    {
        _matchPage.GoToPage(href);
    }

    public void GoToNextPageOfResults()
    {
        _resultsPage.GoToNextPage();
    }

    public void DownloadDemosForMatch()
    {
        _matchPage.DownloadDemosForMatch();
    }

    public IEnumerable<MatchResultDto>? GetParsedMatchResultData(string matchUrl)
    {
        _matchPage.PageHref = matchUrl;
        var hltvGameLink = _matchPage.PageHref;

        _matchPage.GoToPage(matchUrl);
        // Get game info
        var parsedGameInfo = _matchPage.GetInfoForMatches();

        if (!parsedGameInfo.Any())
        {
            _logger.LogError($"Could not parse game info for: {matchUrl}");
            return null;
        }

        return parsedGameInfo;
    }
    
}