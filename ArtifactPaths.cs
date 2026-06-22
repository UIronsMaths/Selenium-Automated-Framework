using System.IO;

/// <summary>
/// Single source of truth for all artifact directory paths.
/// Resolves relative paths against AppContext.BaseDirectory (bin/Debug/net10.0/)
/// to match how ExtentReportManager and LogManager resolve their paths.
/// </summary>
public static class ArtifactPaths
{
    private static readonly string _base = AppContext.BaseDirectory;

    public static string Screenshots =>
        Resolve(ConfigurationManager.Settings.ScreenshotDirectory);

    public static string ExtentReport =>
        Resolve(ConfigurationManager.Settings.ExtentReportDirectory);

    public static string Logs =>
        Resolve(ConfigurationManager.Settings.LogDirectory);

    public static string Allure =>
        Resolve(ConfigurationManager.Settings.AllureDirectory);

    private static string Resolve(string path) =>
        Path.IsPathRooted(path) ? path : Path.Combine(_base, path);
}