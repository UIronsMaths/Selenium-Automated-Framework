using CSharpAxeAccessibility;
using CSharpAxeAccessibility.Utilities;
using Deque.AxeCore.Commons;
using Microsoft.CodeAnalysis;
using OpenQA.Selenium;
namespace CSharpAxeAccessibility.Tests;


[TestFixture]
public class AccessibilityTests : BaseTest
{
    [Test]
    public void LoginPageShouldHaveNoAccessibilityViolations()
    {
        var loginPage = new LoginPage(Driver);
        AxeResult result = AxeAccessibilityUtility.ScanEntirePage(Driver, "Reports/login-page-axe-results.json");
        AxeAccessibilityUtility.AssertNoViolations(result);
    }

    [Test]
    public void LoginFormShouldBeAccessible()
    {
        var loginPage = new LoginPage(Driver);
        IWebElement loginForm = Driver.FindElement(By.ClassName("login-box"));
        AxeResult result = AxeAccessibilityUtility.ScanElement(Driver, loginForm);
        AxeAccessibilityUtility.AssertNoViolations(result);
    }
}
