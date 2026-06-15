using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

public static class WaitUtils
{
    private static int DefaultWaitSeconds => ConfigurationManager.Settings.ExplicitWaitSeconds;

    public static IWebElement WaitForElementVisible(IWebDriver driver, By locator, int? seconds = null)
    {
        var wait = CreateWait(driver, seconds);
        return wait.Until(d =>
        {
            var element = d.FindElement(locator);
            return element.Displayed ? element : null;
        });
    }

    public static IWebElement WaitForElementClickable(IWebDriver driver, By locator, int? seconds = null)
    {
        var wait = CreateWait(driver, seconds);
        return wait.Until(d =>
        {
            var element = d.FindElement(locator);
            return element.Displayed && element.Enabled ? element : null;
        });
    }

    public static bool WaitForElementToDisappear(IWebDriver driver, By locator, int? seconds = null)
    {
        var wait = CreateWait(driver, seconds);
        return wait.Until(d =>
        {
            try
            {
                return !d.FindElement(locator).Displayed;
            }
            catch (NoSuchElementException)
            {
                return true;
            }
        });
    }

    public static bool WaitForUrlToContain(IWebDriver driver, string partialUrl, int? seconds = null)
    {
        var wait = CreateWait(driver, seconds);
        return wait.Until(d => d.Url.Contains(partialUrl, StringComparison.OrdinalIgnoreCase));
    }

    public static bool WaitForTitleToContain(IWebDriver driver, string partialTitle, int? seconds = null)
    {
        var wait = CreateWait(driver, seconds);
        return wait.Until(d => d.Title.Contains(partialTitle, StringComparison.OrdinalIgnoreCase));
    }

    public static IWebElement WaitForElementWithText(IWebDriver driver, By locator, string text, int? seconds = null)
    {
        var wait = CreateWait(driver, seconds);
        return wait.Until(d =>
        {
            var element = d.FindElement(locator);
            return element.Text.Trim() == text ? element : null;
        });
    }

    private static WebDriverWait CreateWait(IWebDriver driver, int? seconds)
    {
        return new WebDriverWait(driver, TimeSpan.FromSeconds(seconds ?? DefaultWaitSeconds));
    }
}