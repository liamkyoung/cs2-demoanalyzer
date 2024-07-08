using CSProsLibrary.Models;
using CSProsLibrary.Services.Interfaces.Scraping;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using CSProsLibrary.Models.Dtos;
using CSProsLibrary.Models.Enums;

namespace CSProsLibrary.Services.Scraping.Pages;

public class PlayerPage
{
    private readonly IWebDriver _driver;
    private string PageHref { get; set; }

    public PlayerPage(IWebDriver driver)
    {
        _driver = driver;
    }
    
    public IWebElement RealName => _driver.FindElement(By.XPath("//div[@class='playerRealname']"));
    public IWebElement GamerTag => _driver.FindElement(By.XPath("//h1[@class='playerNickname']"));
    public IWebElement TeamName => _driver.FindElement(By.XPath("//div[@class='playerInfo']/div[2]/span[2]/span/a"));
    public IWebElement CountryName => _driver.FindElement(By.XPath("//div[@class='playerRealname']/img"));

    public ReadOnlyCollection<IWebElement> PlayerSocials => _driver.FindElements(By.XPath("//div[@class='socialMediaButtons']/a")); // a tags have socials
    public IWebElement ProfileImage => _driver.FindElement(By.XPath("//img[@class='bodyshot-img']"));
    public IWebElement Age => _driver.FindElement(By.XPath("//div[@class='playerInfo']/div[1]/span[2]/span")); // Format: 25 years -> 25
    
    public IWebElement PlayerInfo => _driver.FindElement(By.XPath("//div[@class='playerInfo']"));
    

    public void GoToPage(string pageHref)
    {
        PageHref = pageHref;
        _driver.Navigate().GoToUrl(PageHref);
    }

    public PlayerProfileDto? GetPlayerData()
    {
        var socialMap = ParsePlayerSocials();
        
        var realName = RealName.Text;
        if (string.IsNullOrEmpty(realName))
        {
            Console.WriteLine($"ERROR: Could not get real name for player: {PageHref}");
            return null;
        }

        var gamerTag = GamerTag.Text;
        if (string.IsNullOrEmpty(gamerTag))
        {
            Console.WriteLine($"ERROR: Could not get GamerTag for player: {PageHref}");
            return null;
        }

        var countryName = CountryName.GetAttribute("title");
        if (string.IsNullOrEmpty(countryName))
        {
            Console.WriteLine($"ERROR: Could not get country for player: {gamerTag}");
            return null;
        }
        
        var profileImg = ProfileImage.GetAttribute("src");
        if (string.IsNullOrEmpty(profileImg))
        {
            Console.WriteLine($"ERROR: Could not get profile img for player: {gamerTag}");
            return null;
        }

        var teamName = ParsePlayerTeam();
        if (string.IsNullOrEmpty(teamName))
        {
            Console.WriteLine($"ERROR: Could not find team name for: {gamerTag}");
            return null;
        }

        var parsedAge = ParsePlayerAge();
        if (parsedAge == -1)
        {
            Console.WriteLine($"ERROR: Could not find age for: {gamerTag}");
        }
        
        return new PlayerProfileDto()
        {
            RealName = RealName.Text,
            GamerTag = GamerTag.Text,
            HltvLink = PageHref,
            TeamName = teamName,
            Age = parsedAge,
            ProfileImage = profileImg,
            CountryName = countryName.ToLower(),
            InstagramProfile = socialMap.GetValueOrDefault(SocialMedia.Instagram),
            TwitchProfile = socialMap.GetValueOrDefault(SocialMedia.Twitch),
            TwitterProfile = socialMap.GetValueOrDefault(SocialMedia.Twitter)
        };
    }

    private string? ParsePlayerTeam()
    {
        var rows = PlayerInfo.FindElements(By.XPath(".//div"));
        foreach (var row in rows)
        {
            try
            {
                var leftCol = row.FindElement(By.XPath(".//span[@class='listLeft']"));

                if (leftCol != null)
                {
                    var leftElements = leftCol.FindElements(By.XPath(".//span"));
                    if (leftElements.Any(p => p.Text.ToLower().Contains("team")))
                    {
                        var teamLink = row.FindElement(By.XPath(".//a"));
                        Console.WriteLine($"FOUND TEAM NAME {teamLink?.Text}");
                        return teamLink?.Text;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR COULD NOT FIND TEAM: {e.Message}");
            }
            
        }

        return null;
    }
    
    private int ParsePlayerAge()
    {
        var rows = PlayerInfo.FindElements(By.XPath(".//div"));
        foreach (var row in rows)
        {
            var leftCol = row.FindElement(By.XPath(".//span[@class='listLeft']"));

            if (leftCol?.Text == "Age")
            {
                var ageText = row.FindElement(By.XPath(".//span[@class='listRight']/span")).Text;

                if (ageText == null)
                {
                    return -1;
                }
                
                var cleanedAge = Regex.Replace(ageText, @"\D", "");
                if (string.IsNullOrEmpty(cleanedAge) ||
                    !int.TryParse(cleanedAge, out var parsedAge))
                {
                    return -1;
                }

                return parsedAge;
            }
        }

        return -1;
    }

    private Dictionary<SocialMedia, string> ParsePlayerSocials()
    {
        var socialMap = new Dictionary<SocialMedia, string>();
        
        foreach (var social in PlayerSocials)
        {
            var link = social.GetAttribute("href");

            if (link.Contains("twitter"))
            {
                socialMap.Add(SocialMedia.Twitter, link);
            }
            else if (link.Contains("twitch"))
            {
                socialMap.Add(SocialMedia.Twitch, link);
            }
            else if (link.Contains("instagram"))
            {
                socialMap.Add(SocialMedia.Instagram, link);
            }
            
            // Could add FaceIt
        }

        return socialMap;
    }
}