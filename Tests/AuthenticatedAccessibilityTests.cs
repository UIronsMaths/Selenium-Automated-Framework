using CSharpAxeAccessibility;
using CSharpAxeAccessibility.Utilities;
using Deque.AxeCore.Commons;
using OpenQA.Selenium;
namespace CSharpAxeAccessibility.Tests;

[TestFixture]
public class AuthenticatedAccessibilityTests : BaseTest
{
    [Test]
    public void InventoryPageShouldHaveNoAccessibilityViolations()
    {
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs(settings.Username, settings.Password);
        var inventoryPage = new InventoryPage(Driver);
        
        AxeResult result = AxeAccessibilityUtility.ScanEntirePage(Driver, "Reports/inventory-page-axe-results.json");
        AxeAccessibilityUtility.AssertNoViolations(result);
    }
}