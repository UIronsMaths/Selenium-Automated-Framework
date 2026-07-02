using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using System.Collections.Generic;

public class CartPage
{
    [FindsBy(How = How.ClassName, Using = "title")]
    private IWebElement PageTitle { get; set; } = null!;

    [FindsBy(How = How.ClassName, Using = "cart_item")]
    private IList<IWebElement> CartItems { get; set; } = null!;

    [FindsBy(How = How.Id, Using = "checkout")]
    private IWebElement CheckoutButton { get; set; } = null!;

    [FindsBy(How = How.Id, Using = "continue-shopping")]
    private IWebElement ContinueShoppingButton { get; set; } = null!;

    private readonly IWebDriver Driver;

    public CartPage(IWebDriver driver)
    {
        Driver = driver;
        PageFactory.InitElements(driver, this);
    }

    public bool IsDisplayed() => PageTitle.Displayed;

    public string GetTitle() => PageTitle.Text;

    public int GetItemCount() => CartItems?.Count ?? 0;

    public CheckoutStepOnePage ProceedToCheckout()
    {
        LogManager.Step("Proceeding to checkout");
        CheckoutButton.Click();
        Thread.Sleep(4000);

        var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, System.TimeSpan.FromSeconds(new TestSettings().ExplicitWaitSeconds));
        wait.Until(d => d.Url.Contains("checkout-step-one.html"));

        return new CheckoutStepOnePage(Driver);
    }

    public InventoryPage ContinueShopping()
    {
        LogManager.Step("Continuing shopping from cart");
        ContinueShoppingButton.Click();

        var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, System.TimeSpan.FromSeconds(new TestSettings().ExplicitWaitSeconds));
        wait.Until(d => d.Url.Contains("inventory.html"));

        return new InventoryPage(Driver);
    }
}
