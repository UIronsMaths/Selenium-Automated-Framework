using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Support.UI;
public class InventoryPage
{
    [FindsBy(How = How.ClassName, Using = "title")]
    private IWebElement PageTitle { get; set; } = null!;
    [FindsBy(How = How.ClassName, Using = "inventory_item")]
    private IList<IWebElement> Items { get; set; } = null!;
    [FindsBy(How = How.ClassName, Using = "product_sort_container")]
    private IWebElement SortDropdown { get; set; } = null!;
    [FindsBy(How = How.ClassName, Using = "shopping_cart_link")]
    private IWebElement CartLink { get; set; } = null!;
    [FindsBy(How = How.ClassName, Using = "shopping_cart_badge")]
    private IWebElement? CartBadge { get; set; }
    private readonly IWebDriver Driver;
    public InventoryPage(IWebDriver driver)
    {
        PageFactory.InitElements(driver, this);
        Driver = driver;
    }
    public bool IsDisplayed() => PageTitle.Displayed;
    public string GetTitle() => PageTitle.Text;
    public int GetItemCount() => Items?.Count ?? 0;
    public IEnumerable<string> GetItemNames() => Items == null
        ? Enumerable.Empty<string>()
        : Items.Select(item => item.FindElement(By.ClassName("inventory_item_name")).Text);

    public IEnumerable<decimal> GetItemPrices()
    {
        if (Items == null) yield break;
        foreach (var item in Items)
        {
            var priceEl = item.FindElement(By.ClassName("inventory_item_price"));
            var txt = priceEl.Text.Replace("$", "").Trim();
            if (decimal.TryParse(txt, out var val))
                yield return val;
        }
    }
    public int GetCartBadgeCount()
    {
        try
        {
            if (CartBadge != null && CartBadge.Displayed)
                return int.Parse(CartBadge.Text);
        }
        catch
        {
            // ignore parse errors
        }
        return 0;
    }

    public void AddToCartByName(string itemName)
    {
        LogManager.Step($"Adding item to cart: {itemName}");
        if (Items == null) return;
        foreach (var item in Items)
        {
            try
            {
                var nameEl = item.FindElement(By.ClassName("inventory_item_name"));
                if (nameEl.Text == itemName)
                {
                    var btn = item.FindElement(By.TagName("button"));
                    LogManager.Step($"Button before click: text='{btn.Text}', class='{btn.GetAttribute("class")}'");
                    btn.Click();
                    LogManager.Step($"Button after click: text='{btn.Text}'");
                    Thread.Sleep(4000);

                    var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(new TestSettings().ExplicitWaitSeconds));
                    wait.Until(d => btn.Text == "Remove");
                    return;
                }
            }
            catch
            {
                // ignore and continue
            }
        }
    }

    public void RemoveFromCartByName(string itemName)
    {
        LogManager.Step($"Removing item from cart: {itemName}");
        if (Items == null) return;
        foreach (var item in Items)
        {
            try
            {
                var nameEl = item.FindElement(By.ClassName("inventory_item_name"));
                if (nameEl.Text == itemName)
                {
                    var btn = item.FindElement(By.TagName("button"));
                    btn.Click();

                    var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(new TestSettings().ExplicitWaitSeconds));
                    wait.Until(d => btn.Text == "Add to cart");
                    return;
                }
            }
            catch
            {
                // ignore and continue
            }
        }
    }

    public void SortByNameAsc()
    {
        LogManager.Step("Sorting by name (A to Z)");
        var select = new OpenQA.Selenium.Support.UI.SelectElement(SortDropdown);
        select.SelectByValue("az");
    }

    public void SortByNameDesc()
    {
        LogManager.Step("Sorting by name (Z to A)");
        var select = new OpenQA.Selenium.Support.UI.SelectElement(SortDropdown);
        select.SelectByValue("za");
    }

    public void SortByPriceLowToHigh()
    {
        LogManager.Step("Sorting by price (low to high)");
        var select = new OpenQA.Selenium.Support.UI.SelectElement(SortDropdown);
        select.SelectByValue("lohi");
    }

    public void SortByPriceHighToLow()
    {
        LogManager.Step("Sorting by price (high to low)");
        var select = new OpenQA.Selenium.Support.UI.SelectElement(SortDropdown);
        select.SelectByValue("hilo");
    }

    public CartPage GoToCart()
    {
        LogManager.Step("Navigating to Cart page");
        CartLink.Click();

        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(new TestSettings().ExplicitWaitSeconds));
        wait.Until(d => d.Url.Contains("cart.html"));

        return new CartPage(Driver);
    }
}