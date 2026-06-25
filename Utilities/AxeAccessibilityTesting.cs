using Deque.AxeCore.Commons;
using Deque.AxeCore.Selenium;
using Microsoft.CodeAnalysis;
using OpenQA.Selenium;
namespace CSharpAxeAccessibility.Utilities;

public static class AxeAccessibilityUtility
{
    private static readonly string[] WcagTags =
    [
        "wcag2a",
        "wcag2aa",
        "wcag21a",
        "wcag21aa",
        "wcag22a",
        "wcag22aa"
    ];
    public static AxeResult ScanEntirePage(IWebDriver driver, string? outputFile = null)
    {
        AxeBuilder builder = new AxeBuilder(driver)
        .WithTags(WcagTags);
        if (!string.IsNullOrWhiteSpace(outputFile))
        {
            CreateParentDirectory(outputFile);
            builder.WithOutputFile(outputFile);
        }
        return builder.Analyze();
    }
    public static AxeResult ScanIncludedArea(IWebDriver driver, string cssSelector, string? outputFile = null)
    {
        AxeBuilder builder = new AxeBuilder(driver).Include(cssSelector).WithTags(WcagTags);
        if (!string.IsNullOrWhiteSpace(outputFile))
        {
            CreateParentDirectory(outputFile);
            builder.WithOutputFile(outputFile);
        }
        return builder.Analyze();
    }
    public static AxeResult ScanElement(IWebDriver driver, IWebElement element)
    {
        return new AxeBuilder(driver).WithTags(WcagTags).Analyze(element);
    }
    public static void AssertNoViolations(AxeResult result)
    {
        string report = AccessibilityReportFormatter.Format(result);
        Assert.That(result.Violations, Is.Empty, report);
    }
    private static void CreateParentDirectory(string filePath)
    {
        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}