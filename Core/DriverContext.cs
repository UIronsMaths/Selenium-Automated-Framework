using OpenQA.Selenium;

public static class DriverContext
{
    private static readonly ThreadLocal<IWebDriver> _driver = new ThreadLocal<IWebDriver>();

    public static IWebDriver Driver
    {
        get
        {
            if (_driver.Value == null)
                throw new InvalidOperationException(
                    "WebDriver has not been initialised for this thread. " +
                    "Call DriverContext.SetDriver() before accessing Driver.");
            return _driver.Value;
        }
    }

    public static void SetDriver(IWebDriver driver)
    {
        if (driver == null)
            throw new ArgumentNullException(nameof(driver), "Driver cannot be null.");

        _driver.Value = driver;
    }

    public static bool IsInitialised => _driver.Value != null;

    public static void QuitDriver()
    {
        if (_driver.Value == null) return;

        _driver.Value.Quit();
        _driver.Value.Dispose();
        _driver.Value = null;
    }
}