using Allure.NUnit;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class LoginTests : BaseTest
{
    [Test]
    public void ValidUserCanLogin()
    {
        var loginPage = new LoginPage(Driver);
        var inventoryPage = loginPage.LoginAs(settings.Username, settings.Password);
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
        loginPage.LoginAs(settings.BlankUsername, settings.Password);

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
        loginPage.LoginAs(settings.Username, settings.BlankPassword);

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
        loginPage.LoginAs(settings.BlankUsername, settings.BlankPassword);

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
        loginPage.LoginAs(settings.InvalidUser, settings.Password);

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
        loginPage.LoginAs(settings.Username, settings.WrongPassword);

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
        loginPage.LoginAs(settings.LockedUser, settings.Password);

        var errorMessage = loginPage.GetErrorMessage();
        Assert.Multiple(() =>
        {
            Assert.That(Driver.Url, Does.Not.Contain("inventory.html"));
            Assert.That(errorMessage, Does.Contain("locked out"));
        });
    }

    [Test]
    public void LoginWithSpacesInUsername()
    {
        var loginPage = new LoginPage(Driver);
        loginPage.LoginAs(settings.SpacedUser, settings.Password);

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
        loginPage.LoginAs(settings.Username, settings.SpacedPassword);

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
        loginPage.LoginAs("<script>alert('xss')</script>", settings.Password);

        var errorMessage = loginPage.GetErrorMessage();
        Assert.Multiple(() =>
        {
            Assert.That(Driver.Url, Does.Not.Contain("inventory.html"));
            Assert.That(errorMessage, Is.Not.Empty);
        });
    }
    /*
    [Test]
    [Category("Demo")]
    public void DemoFailingTest_ScreenshotCapture()
    {
        // This test intentionally fails to demonstrate screenshot capture on failure
        var loginPage = new LoginPage(Driver);

        // Try to login with valid credentials
        var inventoryPage = loginPage.LoginAs(settings.Username, settings.Password);

        // Intentionally fail with a wrong assertion to trigger screenshot
        Assert.Multiple(() =>
        {
            Assert.That(Driver.Url, Does.Contain("inventory.html"), "Login should succeed");
            Assert.That(inventoryPage.GetTitle(), Is.EqualTo("Wrong Title"), "This assertion will fail and trigger screenshot capture!");
        });
    }
    */
}