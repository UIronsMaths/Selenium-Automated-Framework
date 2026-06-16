using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

public class CheckoutCompletePage
{
    [FindsBy(How = How.ClassName, Using = "title")]
    private IWebElement PageTitle { get; set; } = null!;

    [FindsBy(How = How.Id, Using = "back-to-products")]
    private IWebElement BackHomeButton { get; set; } = null!;

    private readonly IWebDriver Driver;

    public CheckoutCompletePage(IWebDriver driver)
    {
        Driver = driver;
        PageFactory.InitElements(driver, this);
    }

    public bool IsDisplayed() => PageTitle.Displayed;

    public InventoryPage BackHome()
    {
        LogManager.Step("Clicking Back Home from checkout complete");
        BackHomeButton.Click();

        var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, System.TimeSpan.FromSeconds(new TestSettings().ExplicitWaitSeconds));
        wait.Until(d => d.Url.Contains("inventory.html"));

        return new InventoryPage(Driver);
    }
}
