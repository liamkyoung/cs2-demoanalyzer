using OpenQA.Selenium;

namespace CSProsLibrary.Services.Scraping;

public static class ScrapingHelper
{
    public static Func<IWebDriver, IWebElement> ElementIsClickable(By locator)
    {
        return driver =>
        {
            try
            {
                var element = driver.FindElement(locator);
                return (element != null && element.Displayed && element.Enabled) ? element : null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ElementIsClickable({locator})] Element not found.");
                return null;
            }
        };
    }
    
    public static Func<IWebDriver, IReadOnlyCollection<IWebElement>> PresenceOfAllElementsLocatedBy(By locator)
    {
        return (driver) =>
        {
            try
            {
                var elements = driver.FindElements(locator);
                if (elements.Count > 0)
                {
                    // Console.WriteLine($"Found elements {locator}");
                    return elements;
                }
                
                // Console.WriteLine($"No elements in list {locator}");
                return null;
            }
            catch (NoSuchElementException)
            {
                // Return null to indicate that the WebDriverWait should keep waiting
                Console.WriteLine($"Could not find element {locator}");
                return null;
            }
        };
    }
}