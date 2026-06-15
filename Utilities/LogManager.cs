using Serilog;
using Serilog.Core;

public static class LogManager
{
    private static readonly Lazy<ILogger> _logger = new Lazy<ILogger>(CreateLogger);

    public static ILogger Logger => _logger.Value;

    private static ILogger CreateLogger()
    {
        var settings = ConfigurationManager.Settings;
        var logDirectory = settings.LogDirectory;

        Directory.CreateDirectory(logDirectory);

        var logFilePath = Path.Combine(logDirectory, "test-run-.log");

        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithProperty("Browser", settings.Browser)
            .Enrich.WithProperty("BaseUrl", settings.BaseUrl)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: logFilePath,
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] [{Browser}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 7)
            .CreateLogger();
    }

    public static void TestStarted(string testName)
    {
        Logger.Information("=== TEST STARTED: {TestName} ===", testName);
    }

    public static void TestPassed(string testName)
    {
        Logger.Information("=== TEST PASSED: {TestName} ===", testName);
    }

    public static void TestFailed(string testName, string? message = null)
    {
        Logger.Error("=== TEST FAILED: {TestName} — {Message} ===", testName, message ?? "no message");
    }

    public static void Step(string description)
    {
        Logger.Debug("  Step: {Description}", description);
    }

    public static void Warning(string message)
    {
        Logger.Warning(message);
    }

    public static void Error(string message, Exception? ex = null)
    {
        if (ex is not null)
            Logger.Error(ex, message);
        else
            Logger.Error(message);
    }
}