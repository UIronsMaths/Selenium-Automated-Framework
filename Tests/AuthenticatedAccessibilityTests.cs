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

    [Test]
    public void BasketPageSouldHaveNoAccessibilityViolations()
    {
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs(settings.Username, settings.Password);
        var inventoryPage = new InventoryPage(Driver);

        var first = inventoryPage.GetItemNames().FirstOrDefault();
        Assert.That(first, Is.Not.Null.And.Not.Empty);

        inventoryPage.AddToCartByName(first!);
        LogStep($"Added '{first}' to cart");

        var cart = inventoryPage.GoToCart();

        AxeResult result = AxeAccessibilityUtility.ScanEntirePage(Driver, "Reports/cart-page-axe-results.json");
        AxeAccessibilityUtility.AssertNoViolations(result);
    }

    [Test]
    public void CheckoutPageShouldHaveNoAccessibilityViolations()
    {
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs(settings.Username, settings.Password);
        var inventoryPage = new InventoryPage(Driver);
        var first = inventoryPage.GetItemNames().FirstOrDefault();
        Assert.That(first, Is.Not.Null.And.Not.Empty);
        inventoryPage.AddToCartByName(first!);
        LogStep($"Added '{first}' to cart");
        var cart = inventoryPage.GoToCart();
        var checkoutStepOne = cart.ProceedToCheckout();
        LogStep("On checkout step one");
        AxeResult result = AxeAccessibilityUtility.ScanEntirePage(Driver, "Reports/checkout-step-one-page-axe-results.json");
        AxeAccessibilityUtility.AssertNoViolations(result);
    }

    [Test]
    public void CheckoutCompletionShouldHaveNoAccessibilityViolations()
    {
        LogStep("CompleteCheckoutFlowFinishesAndBackHome: start");

        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs(settings.Username, settings.Password);
        var inventoryPage = new InventoryPage(Driver);

        var first = inventoryPage.GetItemNames().FirstOrDefault();
        Assert.That(first, Is.Not.Null.And.Not.Empty);

        inventoryPage.AddToCartByName(first!);
        var cart = inventoryPage.GoToCart();

        var stepOne = cart.ProceedToCheckout();
        LogStep("Filling checkout details and continuing");
        var stepTwo = stepOne.Continue(settings.ValidBuyerFN, settings.ValidBuyerLN, settings.ValidZipCode);

        LogStep("Finishing checkout");
        var complete = stepTwo.Finish();

        AxeResult result = AxeAccessibilityUtility.ScanEntirePage(Driver, "Reports/checkout-complete-page-axe-results.json");
        AxeAccessibilityUtility.AssertNoViolations(result);
    }
}