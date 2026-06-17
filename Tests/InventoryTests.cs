using Allure.NUnit;
using NUnit.Framework;
using System;
using System.Linq;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class InventoryTests : BaseTest
{
    [Test]
    public void InventoryPageShowsItems()
    {
        LogStep("Starting Inventory page test");

        var loginPage = new LoginPage(Driver);
        LogStep("Logging in with valid credentials");
        loginPage.LoginAs(settings.Username, settings.Password);
        var inventoryPage = new InventoryPage(Driver);

        LogStep("Verifying inventory page is displayed and contains items");

        Assert.Multiple(() =>
        {
            Assert.That(Driver.Url, Does.Contain("inventory.html"));
            Assert.That(inventoryPage.IsDisplayed(), Is.True, "Inventory page should be visible after login");

            var count = inventoryPage.GetItemCount();
            LogStep($"Found {count} inventory items");
            Assert.That(count, Is.GreaterThan(0), "No inventory items were found on the page");
        });

        // Log first few item names for additional detail
        var names = inventoryPage.GetItemNames().ToList();
        var i = 1;
        foreach(var name in names)
        {
            LogStep($"Item {i}: {name}");
            i++;
        }
    }

    [Test]
    public void AddItemToCart()
    {
        LogStep("AddItemToCart: start");
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs(settings.Username, settings.Password);
        var inventoryPage = new InventoryPage(Driver);

        var first = inventoryPage.GetItemNames().FirstOrDefault();
        Assert.That(first, Is.Not.Null.And.Not.Empty, "No item name available to add");

        inventoryPage.AddToCartByName(first!);
        LogStep($"Added '{first}' to cart");

        var badge = inventoryPage.GetCartBadgeCount();
        LogStep($"Cart badge count after adding: {badge}");
        Assert.That(badge, Is.GreaterThan(0), "Cart badge should be incremented after adding an item");
    }

    [Test]
    public void RemoveItemFromCart()
    {
        LogStep("RemoveItemFromCart: start");
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs(settings.Username, settings.Password);
        var inventoryPage = new InventoryPage(Driver);

        var first = inventoryPage.GetItemNames().FirstOrDefault();
        Assert.That(first, Is.Not.Null.And.Not.Empty, "No item name available to remove");

        inventoryPage.AddToCartByName(first!);
        LogStep($"Added '{first}' to cart");

        var badgeAfterAdd = inventoryPage.GetCartBadgeCount();
        Assert.That(badgeAfterAdd, Is.GreaterThan(0));

        inventoryPage.RemoveFromCartByName(first!);
        LogStep($"Removed '{first}' from cart");

        var badgeAfterRemove = inventoryPage.GetCartBadgeCount();
        LogStep($"Cart badge count after removal: {badgeAfterRemove}");
        Assert.That(badgeAfterRemove, Is.LessThanOrEqualTo(0), "Cart badge should be zero after removing the item");
    }

    [Test]
    public void FilterByNameAndVerifyOrder()
    {
        LogStep("FilterByNameAndVerifyOrder: start");
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs(settings.Username, settings.Password);
        var inventoryPage = new InventoryPage(Driver);

        // Sort A->Z
        inventoryPage.SortByNameAsc();
        LogStep("Sorted by name A->Z");
        var names = inventoryPage.GetItemNames().ToList();
        var expected = names.OrderBy(n => n).ToList();
        Assert.That(names, Is.EqualTo(expected), "Names should be in ascending order after sorting A->Z");

        // Sort Z->A
        inventoryPage.SortByNameDesc();
        LogStep("Sorted by name Z->A");
        var namesDesc = inventoryPage.GetItemNames().ToList();
        var expectedDesc = namesDesc.OrderByDescending(n => n).ToList();
        Assert.That(namesDesc, Is.EqualTo(expectedDesc), "Names should be in descending order after sorting Z->A");
    }

    [Test]
    public void FilterByPriceAndVerifyOrder()
    {
        LogStep("FilterByPriceAndVerifyOrder: start");
        var loginPage = new LoginPage(Driver);
        //var inventoryPage = loginPage.LoginAs(settings.Username, settings.Password);
        loginPage.LoginAs(settings.Username, settings.Password);
        var inventoryPage = new InventoryPage(Driver);

        inventoryPage.SortByPriceLowToHigh();
        LogStep("Sorted by price low->high");
        var prices = inventoryPage.GetItemPrices().ToList();
        var expected = prices.OrderBy(p => p).ToList();
        Assert.That(prices, Is.EqualTo(expected), "Prices should be in ascending order after sorting low->high");

        inventoryPage.SortByPriceHighToLow();
        LogStep("Sorted by price high->low");
        var pricesDesc = inventoryPage.GetItemPrices().ToList();
        var expectedDesc = pricesDesc.OrderByDescending(p => p).ToList();
        Assert.That(pricesDesc, Is.EqualTo(expectedDesc), "Prices should be in descending order after sorting high->low");
    }

    [Test]
    public void CanNavigateToCartFromInventory()
    {
        LogStep("CanNavigateToCartFromInventory: start");
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs(settings.Username, settings.Password);
        var inventoryPage = new InventoryPage(Driver);

        // Ensure at least one item in cart to validate cart page contents
        var first = inventoryPage.GetItemNames().FirstOrDefault();
        if (!string.IsNullOrEmpty(first))
        {
            inventoryPage.AddToCartByName(first!);
            LogStep($"Added '{first}' to cart before navigation");
        }

        var cartPage = inventoryPage.GoToCart();
        LogStep("Navigated to cart page");

        Assert.Multiple(() =>
        {
            Assert.That(cartPage.IsDisplayed(), Is.True, "Cart page should be displayed");
            Assert.That(Driver.Url, Does.Contain("cart.html"));
            Assert.That(cartPage.GetItemCount(), Is.GreaterThanOrEqualTo(0));
        });
    }
}
