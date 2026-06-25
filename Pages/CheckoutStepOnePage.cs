using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

public class CheckoutStepOnePage
{
    [FindsBy(How = How.Id, Using = "first-name")]
    private IWebElement FirstName { get; set; } = null!;

    [FindsBy(How = How.Id, Using = "last-name")]
    private IWebElement LastName { get; set; } = null!;

    [FindsBy(How = How.Id, Using = "postal-code")]
    private IWebElement PostalCode { get; set; } = null!;

    [FindsBy(How = How.Id, Using = "continue")]
    private IWebElement ContinueButton { get; set; } = null!;

    [FindsBy(How = How.Id, Using = "cancel")]
    private IWebElement CancelButton { get; set; } = null!;

    [FindsBy(How = How.ClassName, Using = "title")]
    private IWebElement PageTitle { get; set; } = null!;

    private readonly IWebDriver Driver;

    public CheckoutStepOnePage(IWebDriver driver)
    {
        Driver = driver;
        PageFactory.InitElements(driver, this);
    }

    public bool IsDisplayed() => PageTitle.Displayed;

    public CheckoutStepTwoPage Continue(string firstName, string lastName, string postalCode)
    {
        LogManager.Step($"Filling checkout-step-one delivery details");
        FirstName.Clear();
        FirstName.SendKeys(firstName);
        LastName.Clear();
        LastName.SendKeys(lastName);
        PostalCode.Clear();
        PostalCode.SendKeys(postalCode);
        ContinueButton.Click();

        var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, System.TimeSpan.FromSeconds(new TestSettings().ExplicitWaitSeconds));
        wait.Until(d => d.Url.Contains("checkout-step-two.html"));

        return new CheckoutStepTwoPage(Driver);
    }

    public CartPage Cancel()
    {
        LogManager.Step("Cancelling checkout at step one");
        CancelButton.Click();

        var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, System.TimeSpan.FromSeconds(new TestSettings().ExplicitWaitSeconds));
        wait.Until(d => d.Url.Contains("cart.html"));

        return new CartPage(Driver);
    }
}
