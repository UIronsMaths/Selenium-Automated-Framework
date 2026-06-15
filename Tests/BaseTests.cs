using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
public abstract class BaseTest
{
    protected IWebDriver Driver = null!;

    [SetUp]
    public void SetUp()
    {
        Driver = DriverFactory.Create("chrome", false);
        Driver.Navigate().GoToUrl("https://www.saucedemo.com/");
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
                Directory.CreateDirectory("artifacts/screenshots");
                var path = Path.Combine("artifacts/screenshots",
                $"{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                screenshot.SaveAsFile(path);
                TestContext.AddTestAttachment(path);
            }
        }
        finally
        {
            Driver?.Quit();
            Driver?.Dispose();
        }
    }
}