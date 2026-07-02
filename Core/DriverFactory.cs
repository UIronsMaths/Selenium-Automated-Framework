using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
public static class DriverFactory
{
    private static readonly Uri GridHubUri = new Uri("http://localhost:4444/wd/hub");

    public static IWebDriver Create(string browser, bool headless)
    {
        IWebDriver driver = browser.ToLowerInvariant() switch
        {
            "firefox" => new RemoteWebDriver(GridHubUri, CreateFirefoxOptions(headless)),
            "edge" => new RemoteWebDriver(GridHubUri, CreateEdgeOptions(headless)),
            _ => new RemoteWebDriver(GridHubUri, CreateChromeOptions(headless))
        };
        driver.Manage().Window.Maximize();
        return driver;
    }
    private static ChromeOptions CreateChromeOptions(bool headless)
    {
        var options = new ChromeOptions();
        if (headless) options.AddArgument("--headless=new");

        // Performance optimizations
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--no-first-run");
        options.AddArgument("--no-default-browser-check");
        options.AddArgument("--disable-default-apps");
        options.AddArgument("--disable-sync");
        options.AddArgument("--disable-extensions");
        options.AddArgument("--disable-web-resources");

        // Disable notifications and password features
        options.AddArgument("--disable-notifications");
        options.AddArgument("--disable-save-password-bubble");
        options.AddArgument("--disable-password-manager-reauthentication");
        options.AddArgument("--disable-autofill-keyboard-accessory-view");
        options.AddArgument("--disable-component-extensions-with-background-pages");
        options.AddArgument("--disable-features=PasswordLeakDetection");
        options.AddUserProfilePreference("profile.password_manager_leak_detection", false);

        options.AddUserProfilePreference("profile.password_manager_enabled", false);
        options.AddUserProfilePreference("credentials_enable_service", false);
        return options;
    }
    private static EdgeOptions CreateEdgeOptions(bool headless)
    {
        var options = new EdgeOptions();
        if (headless) options.AddArgument("--headless=new");

        // Performance optimizations
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--disable-extensions");
        options.AddArgument("--disable-sync");

        return options;
    }
    private static FirefoxOptions CreateFirefoxOptions(bool headless)
    {
        var options = new FirefoxOptions();
        if (headless) options.AddArgument("-headless");

        // Performance optimizations
        options.SetPreference("dom.max_script_run_time", 30);
        options.SetPreference("browser.sessionstore.max_tabs_undo", 0);
        options.SetPreference("privacy.trackingprotection.enabled", false);

        return options;
    }
}