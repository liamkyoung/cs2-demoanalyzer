using CSProsLibrary.Models.Dtos;

namespace CSProsLibrary.Services.Interfaces.Scraping;

public interface IScrapingService
{
    ParsedSkinInfoDto? GetSkinInfoFromWeaponItemId(long weaponItemId); // HTMLAgilityPack - CSGO Exchange
    Task<PlayerProfileDto?> ParsePlayerProfile(string playerProfileHref); // Selenium
    TeamProfileDto? ParseTeamPage(string teamUrl); // Selenium
    HashSet<string> GetMatchLinks();
    IEnumerable<MatchResultDto>? GetParsedMatchResultData(string matchUrl);
    void GoToPage(string href);
    void GoToMatchPage(string href);
    void DownloadDemosForMatch();
    void GoToNextPageOfResults();
}