using Allure.Net.Commons;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;

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
        var jsonLogFilePath = Path.Combine(logDirectory, "test-run-.json");

        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("OpenQA.Selenium", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Browser", settings.Browser)
            .Enrich.WithProperty("BaseUrl", settings.BaseUrl)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: logFilePath,
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] [{Browser}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 7)
            .WriteTo.File(
                new CompactJsonFormatter(),
                path: jsonLogFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();
    }

    public static void TestStarted(string testName)
    {
        Logger.Information("=== TEST STARTED: {TestName} ===", testName);
        //Logger.Information($"{AllureLifecycle.Instance.ResultsDirectory}");
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

    // New: per-test log scope so every line is tagged with the test name automatically
    public static IDisposable BeginTestScope(string testName)
    {
        return LogContext.PushProperty("TestName", testName);
    }
}