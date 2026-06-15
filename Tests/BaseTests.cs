using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;

public abstract class BaseTest
{
    protected IWebDriver Driver => DriverContext.Driver;

    [SetUp]
    public void SetUp()
    {
        var settings = ConfigurationManager.Settings;

        var driver = DriverFactory.Create(settings.Browser, settings.Headless);
        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(settings.PageLoadTimeoutSeconds);

        DriverContext.SetDriver(driver);
        Driver.Navigate().GoToUrl(settings.BaseUrl);
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                TakeFailureScreenshot();
            }
        }
        finally
        {
            DriverContext.QuitDriver();
        }
    }

    private void TakeFailureScreenshot()
    {
        if (!DriverContext.IsInitialised) return;

        var settings = ConfigurationManager.Settings;
        var screenshotDir = settings.ScreenshotDirectory;

        try
        {
            var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
            Directory.CreateDirectory(screenshotDir);

            var fileName = $"{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            var path = Path.Combine(screenshotDir, fileName);

            screenshot.SaveAsFile(path);
            TestContext.AddTestAttachment(path);
        }
        catch (Exception ex)
        {
            TestContext.WriteLine($"Failed to capture screenshot: {ex.Message}");
        }
    }
}