using Allure.Net.Commons;
using Allure.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using System;
using System.Threading;
using System.Threading.Tasks;

[TestFixture]
[AllureNUnit]
[Parallelizable(ParallelScope.Self)]
public abstract class BaseTest
{
    protected IWebDriver Driver => DriverContext.Driver;

    private string TestName => TestContext.CurrentContext.Test.Name;

    public TestSettings settings = null;

    private IDisposable? _logScope;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        settings = ConfigurationManager.Settings;

        if (settings.ReportType is "extent" or "both")
            ExtentReportManager.Initialise();
    }

    [SetUp]
    public void SetUp()
    {
        _logScope = LogManager.BeginTestScope(TestName);

        var driver = DriverFactory.Create(settings.Browser, settings.Headless);
        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(settings.PageLoadTimeoutSeconds);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(settings.ExplicitWaitSeconds);

        DriverContext.SetDriver(driver);

        LogManager.TestStarted(TestName);

        if (settings.ReportType is "extent" or "both")
            ExtentReportManager.CreateTest(TestName);

        LogManager.Step($"Navigating to {settings.BaseUrl}");
        Driver.Navigate().GoToUrl(settings.BaseUrl);
    }

    [TearDown]
    public void TearDown()
    {
        var status = TestContext.CurrentContext.Result.Outcome.Status;
        var message = TestContext.CurrentContext.Result.Message;

        try
        {
            if (status == TestStatus.Failed)
            {
                HandleTestFailure(message);
            }
            else
            {
                LogManager.TestPassed(TestName);

                if (ConfigurationManager.Settings.ReportType is "extent" or "both")
                    ExtentReportManager.LogPass("Test passed.");
            }
        }
        catch (Exception ex)
        {
            LogManager.Error($"Error during test: {ex.Message}");
        }
        finally
        {
            // Close browser immediately without logging to speed up teardown
            try
            {
                if (DriverContext.IsInitialised)
                {
                    DriverContext.QuitDriver();
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error closing browser: {ex.Message}");
            }

            // Flush reports asynchronously to avoid blocking test completion
            try
            {
                ExtentReportManager.Flush();
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error flushing report: {ex.Message}");
            }

            // Dispose the per-test log scope last so failure/teardown logs above still get tagged
            _logScope?.Dispose();
            _logScope = null;
        }
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        ExtentReportManager.Flush();
    }

    private void HandleTestFailure(string? message)
    {
        LogManager.TestFailed(TestName, message);

        if (!DriverContext.IsInitialised) return;

        var screenshotPath = ScreenshotUtils.CaptureOnFailure(Driver, TestName);

        if (screenshotPath is not null)
        {
            LogManager.Step($"Screenshot saved to: {screenshotPath}");
            TestContext.AddTestAttachment(screenshotPath);

            if (settings.ReportType is "extent" or "both")
                ExtentReportManager.LogFail(message ?? "Test failed.", screenshotPath);
            if(settings.ReportType is "allure" or "both")
                AllureApi.AddAttachment("Screenshot", "image/png", screenshotPath);
        }
        else
        {
            // Fall back to base64 if file capture failed
            var base64 = ScreenshotUtils.CaptureAsBase64(Driver);

            if (settings.ReportType is "extent" or "both")
                ExtentReportManager.LogFailWithBase64(message ?? "Test failed.", base64);
            if (settings.ReportType is "allure" or "both")
                AllureApi.AddAttachment("Screenshot", "image/png", Convert.FromBase64String(base64));
        }
    }

    // Helper for subclasses to log steps to both Serilog and Extent in one call
    protected void LogStep(string description)
    {
        LogManager.Step(description);

        if (settings.ReportType is "extent" or "both")
            ExtentReportManager.LogStep(description);
        if(settings.ReportType is "allure" or "both")
        {
            AllureApi.Step(description);
        }
    }
}