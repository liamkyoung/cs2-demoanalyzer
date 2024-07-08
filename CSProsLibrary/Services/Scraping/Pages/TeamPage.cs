using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using CSProsLibrary.Models;
using CSProsLibrary.Models.Dtos;
using CSProsLibrary.Services.Interfaces.Scraping;
using OpenQA.Selenium;

namespace CSProsLibrary.Services.Scraping.Pages;

public class TeamPage
{
    private readonly IWebDriver _driver;
    public string? PageHref;

    public TeamPage(IWebDriver driver)
    {
        _driver = driver;
    }

    public void GoToPage(string pageHref)
    {
        PageHref = pageHref;
        _driver.Navigate().GoToUrl(PageHref);
    }
    // Example Properties:
    public IWebElement Name => _driver.FindElement(By.XPath("//div[@class='profile-team-info']/h1"));
    public ReadOnlyCollection<IWebElement> Players => _driver.FindElements(By.XPath("//div[@class='teamProfile']/div[1]/div[1]/a")); // Players by Hltv link
    public IWebElement Country => _driver.FindElement(By.XPath("//div[@class='profile-team-info']/div/img")); // getAttribute('title')

    // public string HltvProfile => PageHref;
    public IWebElement HltvRanking => _driver.FindElement(By.XPath("//div[@class='profile-team-stats-container']/div[1]/span/a")); // Format: #5 -> 5
    public ReadOnlyCollection<IWebElement> TeamStats => _driver.FindElements(By.XPath("//div[@class='profile-team-stat']"));
    
    public IWebElement ProfileImgDefault => _driver.FindElement(By.XPath("//img[@class='teamlogo']"));

    public IWebElement ProfileImgDarkMode => _driver.FindElement(By.XPath("//img[@class='teamlogo night-only']"));
    
    public TeamProfileDto? GetTeamInfo()
    {
        try
        {
            var name = Name.Text;
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine($"ERROR: Name empty for {PageHref}");
                return null;
            }

            var countryName = Country.GetAttribute("title");
            if (string.IsNullOrEmpty(countryName))
            {
                Console.WriteLine($"ERROR: Country name empty for {Name.Text}");
                return null;
            }

            var coachName = ParseCoachName();
            if (string.IsNullOrEmpty(coachName))
            {
                Console.WriteLine($"ERROR: Coach name empty for {name}");
                return null;
            }

            var imageSrc = ParseImageSrc();

            var rank = ParseHltvRank();
            rank ??= -1; // Not ranked

            var playerLinks = new List<string>();
            foreach (var player in Players)
            {
                var playerLink = player.GetAttribute("href");

                if (playerLink == null)
                {
                    Console.WriteLine($"ERROR: Player Link is null. Team: {name}");
                    continue;
                }

                playerLinks.Add(playerLink);
            }

            return new TeamProfileDto()
            {
                Name = name,
                CoachName = coachName,
                PlayerHltvLinks = playerLinks,
                CountryName = countryName,
                HltvProfile = PageHref!,
                HltvRanking = rank.Value,
                ImageSrc = imageSrc
            };
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to parse team page");
        }


        return null;
    }

    private string? ParseCoachName()
    {
        foreach (var row in TeamStats)
        {
            var title = row.FindElement(By.XPath(".//b"));

            if (title != null && title.Text == "Coach")
            {
                return row.FindElement(By.XPath(".//a/span")).Text.Replace("'", ""); // Could be affected if coach name has ' in it.
            }
        }

        return null;
    }

    private int? ParseHltvRank()
    {
        var ranking = HltvRanking.Text;
        int rank = 0;
        if (string.IsNullOrEmpty(ranking))
        {
            return null;
        }
        else
        {
            var formattedRanking = Regex.Replace(ranking, @"\D", "");
            int.TryParse(formattedRanking, out int rankingValue);
            rank = rankingValue;
        }

        return rank;
    }

    private string? ParseImageSrc()
    {
        try
        {
            var profileImg = ProfileImgDefault.GetAttribute("src");
            return profileImg;
        }
        catch (Exception e) { }

        try
        {
            var profileImg = ProfileImgDarkMode.GetAttribute("src");
            return profileImg;
        }
        catch (Exception e)
        {
            Console.WriteLine("Img for Team Does not exist");
        }

        return null;
    }
}