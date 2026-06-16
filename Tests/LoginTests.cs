using NUnit.Framework;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
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

    [Test]
    public void LoginWithEmptyUsername()
    {
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs("", "secret_sauce");

        var errorMessage = loginPage.GetErrorMessage();
        Assert.Multiple(() =>
        {
            Assert.That(Driver.Url, Does.Not.Contain("inventory.html"));
            Assert.That(errorMessage, Does.Contain("Username is required"));
        });
    }

    [Test]
    public void LoginWithEmptyPassword()
    {
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs("standard_user", "");

        var errorMessage = loginPage.GetErrorMessage();
        Assert.Multiple(() =>
        {
            Assert.That(Driver.Url, Does.Not.Contain("inventory.html"));
            Assert.That(errorMessage, Does.Contain("Password is required"));
        });
    }

    [Test]
    public void LoginWithBothUsernameAndPasswordEmpty()
    {
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs("", "");

        var errorMessage = loginPage.GetErrorMessage();
        Assert.Multiple(() =>
        {
            Assert.That(Driver.Url, Does.Not.Contain("inventory.html"));
            Assert.That(errorMessage, Does.Contain("Username is required"));
        });
    }

    [Test]
    public void LoginWithInvalidUsername()
    {
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs("invalid_user", "secret_sauce");

        var errorMessage = loginPage.GetErrorMessage();
        Assert.Multiple(() =>
        {
            Assert.That(Driver.Url, Does.Not.Contain("inventory.html"));
            Assert.That(errorMessage, Does.Contain("Username and password do not match"));
        });
    }

    [Test]
    public void LoginWithInvalidPassword()
    {
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs("standard_user", "wrong_password");

        var errorMessage = loginPage.GetErrorMessage();
        Assert.Multiple(() =>
        {
            Assert.That(Driver.Url, Does.Not.Contain("inventory.html"));
            Assert.That(errorMessage, Does.Contain("Username and password do not match"));
        });
    }

    [Test]
    public void LoginWithLockedOutUser()
    {
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs("locked_out_user", "secret_sauce");

        var errorMessage = loginPage.GetErrorMessage();
        Assert.Multiple(() =>
        {
            Assert.That(Driver.Url, Does.Not.Contain("inventory.html"));
            Assert.That(errorMessage, Does.Contain("locked out"));
        });
    }

    [Test]
    public void LoginWithSpecialCharactersInUsername()
    {
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs("user@#$%", "secret_sauce");

        var errorMessage = loginPage.GetErrorMessage();
        Assert.Multiple(() =>
        {
            Assert.That(Driver.Url, Does.Not.Contain("inventory.html"));
            Assert.That(errorMessage, Is.Not.Empty);
        });
    }

    [Test]
    public void LoginWithSpacesInUsername()
    {
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs(" standard_user ", "secret_sauce");

        var errorMessage = loginPage.GetErrorMessage();
        Assert.Multiple(() =>
        {
            Assert.That(Driver.Url, Does.Not.Contain("inventory.html"));
            Assert.That(errorMessage, Does.Contain("Username and password do not match"));
        });
    }

    [Test]
    public void LoginWithSpacesInPassword()
    {
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs("standard_user", " secret_sauce ");

        var errorMessage = loginPage.GetErrorMessage();
        Assert.Multiple(() =>
        {
            Assert.That(Driver.Url, Does.Not.Contain("inventory.html"));
            Assert.That(errorMessage, Does.Contain("Username and password do not match"));
        });
    }

    [Test]
    public void LoginWithSQLInjectionAttempt()
    {
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs("' OR '1'='1", "' OR '1'='1");

        var errorMessage = loginPage.GetErrorMessage();
        Assert.Multiple(() =>
        {
            Assert.That(Driver.Url, Does.Not.Contain("inventory.html"));
            Assert.That(errorMessage, Is.Not.Empty);
        });
    }

    [Test]
    public void LoginWithXSSAttempt()
    {
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs("<script>alert('xss')</script>", "secret_sauce");

        var errorMessage = loginPage.GetErrorMessage();
        Assert.Multiple(() =>
        {
            Assert.That(Driver.Url, Does.Not.Contain("inventory.html"));
            Assert.That(errorMessage, Is.Not.Empty);
        });
    }
}