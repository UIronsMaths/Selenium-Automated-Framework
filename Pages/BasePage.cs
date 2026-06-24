using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;

public abstract class BasePage
{
    protected readonly IWebDriver Driver;
    protected readonly WebDriverWait Wait;

    protected BasePage(IWebDriver driver, int explicitWaitSeconds = 10)
    {
        Driver = driver;
        Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(explicitWaitSeconds));
        PageFactory.InitElements(driver, this);
    }

    protected IWebElement WaitForElement(By locator) => Wait.Until(d => d.FindElement(locator));

    protected bool IsElementVisible(By locator)
    {
        try
        {
            return Driver.FindElement(locator).Displayed;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    protected void NavigateTo(string url)
    {
        LogManager.Step($"Navigating to: {url}");
        Driver.Navigate().GoToUrl(url);
    }

    public string GetPageTitle() => Driver.Title;

    public string GetCurrentUrl() => Driver.Url;
}