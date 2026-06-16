using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

public class CheckoutStepTwoPage
{
    [FindsBy(How = How.Id, Using = "finish")]
    private IWebElement FinishButton { get; set; } = null!;

    [FindsBy(How = How.Id, Using = "cancel")]
    private IWebElement CancelButton { get; set; } = null!;

    [FindsBy(How = How.ClassName, Using = "title")]
    private IWebElement PageTitle { get; set; } = null!;

    private readonly IWebDriver Driver;

    public CheckoutStepTwoPage(IWebDriver driver)
    {
        Driver = driver;
        PageFactory.InitElements(driver, this);
    }

    public bool IsDisplayed() => PageTitle.Displayed;

    public CheckoutCompletePage Finish()
    {
        LogManager.Step("Finishing checkout (step two)");
        FinishButton.Click();

        var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, System.TimeSpan.FromSeconds(new TestSettings().ExplicitWaitSeconds));
        wait.Until(d => d.Url.Contains("checkout-complete.html"));

        return new CheckoutCompletePage(Driver);
    }

    public CartPage Cancel()
    {
        LogManager.Step("Cancelling checkout at step two");
        CancelButton.Click();

        var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, System.TimeSpan.FromSeconds(new TestSettings().ExplicitWaitSeconds));
        wait.Until(d => d.Url.Contains("cart.html"));

        return new CartPage(Driver);
    }
}
