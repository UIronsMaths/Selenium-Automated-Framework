using NUnit.Framework;
using System.Linq;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class CheckoutTests : BaseTest
{
    [Test]
    public void ContinueShoppingFromCartReturnsToInventory()
    {
        LogStep("ContinueShoppingFromCartReturnsToInventory: start");

        var loginPage = new LoginPage(Driver);
        var inventoryPage = loginPage.LoginAs(settings.Username, settings.Password);

        var first = inventoryPage.GetItemNames().FirstOrDefault();
        Assert.That(first, Is.Not.Null.And.Not.Empty);

        inventoryPage.AddToCartByName(first!);
        LogStep($"Added '{first}' to cart");

        var cart = inventoryPage.GoToCart();
        LogStep("At cart page");

        var inv = cart.ContinueShopping();
        LogStep("Clicked Continue Shopping");

        Assert.Multiple(() =>
        {
            Assert.That(inv.IsDisplayed(), Is.True);
            Assert.That(Driver.Url, Does.Contain("inventory.html"));
        });
    }

    [Test]
    public void CancelCheckoutAtStepOneReturnsToCart()
    {
        LogStep("CancelCheckoutAtStepOneReturnsToCart: start");

        var loginPage = new LoginPage(Driver);
        var inventoryPage = loginPage.LoginAs(settings.Username, settings.Password);

        var first = inventoryPage.GetItemNames().FirstOrDefault();
        Assert.That(first, Is.Not.Null.And.Not.Empty);

        inventoryPage.AddToCartByName(first!);
        var cart = inventoryPage.GoToCart();

        var stepOne = cart.ProceedToCheckout();
        LogStep("On checkout step one");

        var returnedCart = stepOne.Cancel();
        LogStep("Cancelled checkout at step one");

        Assert.Multiple(() =>
        {
            Assert.That(returnedCart.GetItemCount(), Is.GreaterThanOrEqualTo(0));
            Assert.That(Driver.Url, Does.Contain("cart.html"));
        });
    }

    [Test]
    public void CompleteCheckoutFlowFinishesAndBackHome()
    {
        LogStep("CompleteCheckoutFlowFinishesAndBackHome: start");

        var loginPage = new LoginPage(Driver);
        var inventoryPage = loginPage.LoginAs(settings.Username, settings.Password);

        var first = inventoryPage.GetItemNames().FirstOrDefault();
        Assert.That(first, Is.Not.Null.And.Not.Empty);

        inventoryPage.AddToCartByName(first!);
        var cart = inventoryPage.GoToCart();

        var stepOne = cart.ProceedToCheckout();
        LogStep("Filling checkout details and continuing");
        var stepTwo = stepOne.Continue("Test", "User", "90210");

        LogStep("Finishing checkout");
        var complete = stepTwo.Finish();

        Assert.Multiple(() =>
        {
            Assert.That(complete.IsDisplayed(), Is.True);
            Assert.That(Driver.Url, Does.Contain("checkout-complete.html"));
        });

        var inventory = complete.BackHome();
        LogStep("Clicked Back Home after complete");

        Assert.That(inventory.IsDisplayed(), Is.True);
    }
}
