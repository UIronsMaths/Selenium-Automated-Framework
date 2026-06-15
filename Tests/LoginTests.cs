using NUnit.Framework;
[TestFixture]
public class LoginTests : BaseTest
{
    [Test]
    public void ValidUserCanLogin()
    {
        var loginPage = new LoginPage(Driver);
        var inventoryPage = loginPage.LoginAs("standard_user", "secret_sauce");
        Assert.Multiple(() =>
        {
            Assert.That(Driver.Url, Does.Contain("inventory.html"));
            Assert.That(inventoryPage.IsDisplayed(), Is.True);
            Assert.That(inventoryPage.GetTitle(), Is.EqualTo("Products"));
        });
    }
}