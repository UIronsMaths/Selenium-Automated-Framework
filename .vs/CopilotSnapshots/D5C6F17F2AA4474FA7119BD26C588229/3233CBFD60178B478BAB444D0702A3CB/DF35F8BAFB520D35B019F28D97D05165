using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
public static class DriverFactory
{
    public static IWebDriver Create(string browser, bool headless)
    {
        IWebDriver driver = browser.ToLowerInvariant() switch
        {
            "firefox" => new FirefoxDriver(CreateFirefoxOptions(headless)),
            "edge" => new EdgeDriver(CreateEdgeOptions(headless)),
            _ => new ChromeDriver(CreateChromeOptions(headless))
        };
        driver.Manage().Window.Maximize();
        return driver;
    }
    private static ChromeOptions CreateChromeOptions(bool headless)
    {
        var options = new ChromeOptions();
        if (headless) options.AddArgument("--headless=new");
        options.AddArgument("--disable-notifications");
        return options;
    }
    private static EdgeOptions CreateEdgeOptions(bool headless)
    {
        var options = new EdgeOptions();
        if (headless) options.AddArgument("--headless=new");
        return options;
    }
    private static FirefoxOptions CreateFirefoxOptions(bool headless)
    {
        var options = new FirefoxOptions();
        if (headless) options.AddArgument("-headless");
        return options;
    }
}