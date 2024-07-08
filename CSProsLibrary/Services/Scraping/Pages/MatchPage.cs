using System.Globalization;
using CSProsLibrary.Models;
using System.Text.RegularExpressions;
using CSProsLibrary.Models.Dtos;
using CSProsLibrary.Services.Interfaces.Scraping;
using CSProsLibrary.Services.Scraping;
using CSProsLibrary.Models.Dtos;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CSProsLibrary.Services.Scraping.Pages;

public class MatchPage
{
    private readonly IWebDriver _driver;
    private readonly ILogger<MatchPage> _logger;
    private const string DEMO_DOWNLOAD_XPATH = "//a[contains(@href, '/download/demo')]";
    private const int TIMEOUT_SECONDS = 30;
    public string PageHref;

    public MatchPage(IWebDriver driver, ILogger<MatchPage> logger)
    {
        _driver = driver;
        _logger = logger;
    }
    
    // Example Properties:
    // public string DemoDownloadLink => driver.FindElement(By.Id());
    public IWebElement TeamAProfileLink => _driver.FindElement(By.XPath("//div[@class='team1-gradient']/a"));
    public IWebElement TeamBProfileLink => _driver.FindElement(By.XPath("//div[@class='team2-gradient']/a"));
    public IWebElement TeamAName => _driver.FindElement(By.XPath("//div[@class='team1-gradient']/a/div[@class='teamName']"));
    public IWebElement TeamBName => _driver.FindElement(By.XPath("//div[@class='team2-gradient']/a/div[@class='teamName']"));
    public IWebElement GameTime => _driver.FindElement(By.XPath("//div[@class='time']"));
    
    public IEnumerable<IWebElement> Maps => _driver.FindElements(By.XPath("//div[@class='mapholder']"));
    
    public IWebElement Tournament => _driver.FindElement(By.XPath("//div[@class='timeAndEvent']/div[3]/a"));
    public int? GameId => GetGameId();
    
    public IReadOnlyCollection<IWebElement> DemoDownloadLinks => new WebDriverWait(_driver, TimeSpan.FromSeconds(10)).Until(
        ScrapingHelper.PresenceOfAllElementsLocatedBy(By.XPath(DEMO_DOWNLOAD_XPATH)));
    

    public void GoToPage(string matchUrl)
    {
        PageHref = matchUrl;
        _driver.Navigate().GoToUrl(PageHref);
    }
    
    public bool DownloadDemosForMatch()
    {
        if (string.IsNullOrEmpty(PageHref))
        {
            _logger.LogError("Page was not set for match. Cannot download demo.");
            return false;
        }
        
        try
        {
            var demoLinks = DemoDownloadLinks;

            if (demoLinks.Count == 1)
            {
                var demoLink = demoLinks.First().GetAttribute("href");
            
                var window = _driver.CurrentWindowHandle;
                _driver.SwitchTo().NewWindow(WindowType.Tab);
                
                try
                {
                    Console.WriteLine($"Downloading Demo: {demoLink}");
                    _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(TIMEOUT_SECONDS);
                    _driver.Navigate().GoToUrl(demoLink);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception when downloading demo: {e}");
                    _driver.Close();
                    _driver.SwitchTo().Window(window);
                }
            }
            else
            {
                Console.WriteLine("Not a demo link on page...");
            }
            
        }
        catch (Exception e)
        {
            Console.WriteLine("Could not find link to download demos on page");
        }

        return false;
    }

    public int? GetGameId()
    {
        Match match = Regex.Match( PageHref, @"matches/(\d+)/");
        
        if (match.Success)
        {
            var matchId = match.Groups[1].Value; // TODO: Make cleaner / less error prone

            if (int.TryParse(matchId, out var id))
            {
                return id;
            }
        }
        
        return null;
    }

    public string[]? GetTeamProfileLinks()
    {
        var teamALink = TeamAProfileLink.GetAttribute("href");
        var teamBLink = TeamBProfileLink.GetAttribute("href");

        if (string.IsNullOrEmpty(teamALink) || string.IsNullOrEmpty(teamBLink))
        {
            Console.WriteLine("Could not get links for teams");
            return null;
        }

        return new[] { teamALink, teamBLink };
    }

    public IEnumerable<MatchResultDto> GetInfoForMatches()
    {
        try
        {
            var id = GetGameId();
           

            if (id == null)
            {
                _logger.LogError($"Couldn't get GameId");
                return Enumerable.Empty<MatchResultDto>();
            }
            
            var unixTime = GameTime.GetAttribute("data-unix");
            if (!long.TryParse(unixTime, out var unixTimeMs))
            {
                _logger.LogError($"Couldn't parse time of game");
                return Enumerable.Empty<MatchResultDto>();
            }

            if (string.IsNullOrEmpty(TeamAName.Text) || 
                string.IsNullOrEmpty(TeamBName.Text) ||
                string.IsNullOrEmpty(Tournament.Text)) // TODO: Will need to support BO3's
            {
                _logger.LogError($"Couldn't get team name/tournament name");
                return Enumerable.Empty<MatchResultDto>();
            }

            if (!Maps.Any())
            {
                _logger.LogError($"Couldn't find any maps on page");
                return Enumerable.Empty<MatchResultDto>();
            }
            
            var teamLinks = GetTeamProfileLinks();
            if (teamLinks == null)
            {
                _logger.LogError($"Couldn't get profile links for each team");
                return Enumerable.Empty<MatchResultDto>();
            }

            var matches = new List<MatchResultDto>();
            foreach (var map in Maps)
            {
                var teamScores = map.FindElements(By.XPath(".//div[@class='results-team-score']"));

                if (teamScores.Count() != 2)
                {
                    _logger.LogError($"Couldn't get team scores ({PageHref}) | Scores: {teamScores.Count()}");
                    continue;
                }

                if (!int.TryParse(teamScores.First().Text, out var teamAScore) ||
                    !int.TryParse(teamScores.Last().Text, out var teamBScore))
                {
                    _logger.LogError($"Couldn't find team scores ({PageHref}) | A: {teamScores.First().Text} | B: {teamScores.Last().Text}");
                    continue;
                }

                var mapName = map.FindElement(By.XPath(".//div[@class='mapname']")).Text;
            
                matches.Add(new MatchResultDto()
                {
                    Id = id.Value,
                    TeamAName = TeamAName.Text,
                    TeamBName = TeamBName.Text,
                    TeamAScore = teamAScore,
                    TeamBScore = teamBScore,
                    TeamAProfileLink = teamLinks[0],
                    TeamBProfileLink = teamLinks[1],
                    StartedAt = DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMs),
                    HltvLink = PageHref,
                    HltvStars = 1, // TODO
                    NumberOfMaps = Maps.Count(),
                    MapName = mapName,
                    TournamentName = Tournament.Text
                });
            }
            
            _logger.LogInformation($"[BO3/BO5 Check] Created list of {matches.Count} matches");
            return matches;
        }
        catch (Exception e)
        {
            _logger.LogError("Could not create matches.");
        }
        

        return Enumerable.Empty<MatchResultDto>();
    }
}