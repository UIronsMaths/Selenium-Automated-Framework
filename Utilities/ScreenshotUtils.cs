using OpenQA.Selenium;

public static class ScreenshotUtils
{
    private static string DefaultDirectory => ConfigurationManager.Settings.ScreenshotDirectory;

    public static string? CaptureOnFailure(IWebDriver driver, string testName)
    {
        return Capture(driver, testName, DefaultDirectory);
    }

    public static string? Capture(IWebDriver driver, string testName, string? directory = null)
    {
        var targetDir = directory ?? DefaultDirectory;

        try
        {
            var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            Directory.CreateDirectory(targetDir);

            var fileName = $"{Sanitise(testName)}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            var fullPath = Path.Combine(targetDir, fileName);

            screenshot.SaveAsFile(fullPath);
            return fullPath;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ScreenshotUtils] Failed to capture screenshot for '{testName}': {ex.Message}");
            return null;
        }
    }

    public static string? CaptureAsBase64(IWebDriver driver)
    {
        try
        {
            return ((ITakesScreenshot)driver).GetScreenshot().AsBase64EncodedString;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ScreenshotUtils] Failed to capture base64 screenshot: {ex.Message}");
            return null;
        }
    }

    private static string Sanitise(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Concat(name.Select(c => invalid.Contains(c) ? '_' : c));
    }
}