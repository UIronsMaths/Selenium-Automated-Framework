using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
public class InventoryPage
{
    [FindsBy(How = How.ClassName, Using = "title")]
    private IWebElement PageTitle { get; set; } = null!;
    public InventoryPage(IWebDriver driver)
    {
        PageFactory.InitElements(driver, this);
    }
    public bool IsDisplayed() => PageTitle.Displayed;
    public string GetTitle() => PageTitle.Text;
}