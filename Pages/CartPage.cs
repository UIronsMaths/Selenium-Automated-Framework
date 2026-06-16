using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using System.Collections.Generic;

public class CartPage
{
    [FindsBy(How = How.ClassName, Using = "title")]
    private IWebElement PageTitle { get; set; } = null!;

    [FindsBy(How = How.ClassName, Using = "cart_item")]
    private IList<IWebElement> CartItems { get; set; } = null!;

    public CartPage(IWebDriver driver)
    {
        PageFactory.InitElements(driver, this);
    }

    public bool IsDisplayed() => PageTitle.Displayed;

    public string GetTitle() => PageTitle.Text;

    public int GetItemCount() => CartItems?.Count ?? 0;
}
