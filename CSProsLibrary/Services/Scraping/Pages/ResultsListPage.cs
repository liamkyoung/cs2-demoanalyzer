using CSProsLibrary.Models.Dtos;
using OpenQA.Selenium.Support.UI;
using CSProsLibrary.Services.Interfaces.Scraping;
using OpenQA.Selenium;

namespace CSProsLibrary.Services.Scraping.Pages;

public class ResultsListPage
{
    private readonly IWebDriver _driver;
    private const string MATCH_URL_XPATH = "//div[@class='result-con']//a[contains(@href, '/matches/')]";
    private const string RESULT_PAGE_URL = "https://www.hltv.org/results";
    private const string COOKIE_DECLINE_ID = "CybotCookiebotDialogBodyButtonDecline";
    private int _offset = 0;
    private int _hltvStars = 2;

    public string PageUrl => $"{RESULT_PAGE_URL}?stars={_hltvStars}&offset={_offset}";
    
    public ResultsListPage(IWebDriver driver)
    {
        _driver = driver;
    }
    
    private IReadOnlyCollection<IWebElement> MatchLinks => new WebDriverWait(_driver, TimeSpan.FromSeconds(10))
        .Until(ScrapingHelper.PresenceOfAllElementsLocatedBy(By.XPath(MATCH_URL_XPATH)));
    
    // public IWebElement BackArrow
    // public IWebElement ForwardArrow
    
    // public void ClickOnMatch() { driver }
    // public string GoBackwards() { driver }
    // public string GoForwards() { driver }

    public void GoToNextPage()
    {
        _offset += 100;
        _driver.Navigate().GoToUrl(PageUrl);
    }
    
    public void GoBackPage()
    {
        _offset = Math.Max(0, _offset - 100);
        _driver.Navigate().GoToUrl(PageUrl);
    }

    public void SetHltvStars(int stars)
    {
        if (stars >= 0 && stars <= 5)
        {
            _hltvStars = stars;
            _offset = 0;
        }
    }

    public void GoToPage()
    {
        _driver.Navigate().GoToUrl(PageUrl);
        DeclineCookies();
    }

    private void DeclineCookies()
    {
        try
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var clickableCookieButton = wait.Until(ScrapingHelper.ElementIsClickable(By.Id(COOKIE_DECLINE_ID))); // May need to wait until clickable
            clickableCookieButton.Click();
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to decline cookies...");
        }
    }

    public HashSet<string> GetMatches()
    {
        if (_driver.Url != PageUrl)
        {
            GoToPage();
        }
        
        var matchUrls = new HashSet<string>();

        try
        {
            Console.WriteLine("Getting match links...");
            
            for (var i = 0; i < MatchLinks.Count; i++)
            {
                var matchLink = MatchLinks.ElementAt(i).GetAttribute("href");

                if (!string.IsNullOrEmpty(matchLink))
                {
                    matchUrls.Add(matchLink);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error getting match links");
        }
        
        return matchUrls; 
    }
}
