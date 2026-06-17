using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
public class LoginPage
{
    private readonly IWebDriver driver;
    [FindsBy(How = How.Id, Using = "user-name")]
    private IWebElement Username { get; set; } = null!;
    [FindsBy(How = How.Id, Using = "password")]
    private IWebElement Password { get; set; } = null!;
    [FindsBy(How = How.Id, Using = "login-button")]
    private IWebElement LoginButton { get; set; } = null!;
    [FindsBy(How = How.CssSelector, Using = "[data-test='error']")]
    private IWebElement ErrorMessage { get; set; } = null!;
    public LoginPage(IWebDriver driver)
    {
        this.driver = driver;
        PageFactory.InitElements(driver, this);
    }
    public void LoginAs(string username, string password)
    {
        LogManager.Step("Attempting login");
        Username.Clear();
        Username.SendKeys(username);
        Password.Clear();
        Password.SendKeys(password);
        LoginButton.Click();
        HandleBrowserAlert();
        //return new InventoryPage(driver);
    }
    private void HandleBrowserAlert()
    {
        try
        {
            // Wait briefly for the alert to appear
            System.Threading.Thread.Sleep(1000);

            var alert = driver.SwitchTo().Alert();
            alert.Accept();
        }
        catch (NoAlertPresentException)
        {
            // No alert present, continue normally
        }
        catch
        {
            // Other exceptions, log but continue
        }
    }
    public string GetErrorMessage()
    {
        try
        {
            // Wait a bit for error message to appear
            System.Threading.Thread.Sleep(500);
            if (ErrorMessage != null && ErrorMessage.Displayed)
            {
                return ErrorMessage.Text;
            }
        }
        catch (NoSuchElementException)
        {
            // Error element not found
        }
        catch
        {
            // Other exceptions
        }
        return string.Empty;
    }
}