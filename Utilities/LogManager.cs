using Allure.Net.Commons;
using Serilog;
using Serilog.Context;
using System.Threading;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;

public static class LogManager
{
    private static readonly Lazy<ILogger> _logger = new Lazy<ILogger>(CreateLogger);

    public static ILogger Logger => _logger.Value;

    // Per-test expected value stored in AsyncLocal so it is isolated per logical test execution
    private static readonly AsyncLocal<string?> _expected = new AsyncLocal<string?>();

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
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [Test:{TestName}] [Id:{TestId}] [T:{ThreadId}] [B:{Browser}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: logFilePath,
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] [Test:{TestName}] [Id:{TestId}] [T:{ThreadId}] [B:{Browser}] {Message:lj}{NewLine}{Exception}",
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
        // Back-compat overload: push only TestName. Prefer BeginTestScope(testName, testId, browser)
        return LogContext.PushProperty("TestName", testName);
    }

    // New overload: push multiple properties (TestName, TestId, ThreadId, Browser) so all logs
    // automatically include test metadata. Returns an IDisposable that will pop all properties.
    public static IDisposable BeginTestScope(string testName, string testId, string browser)
    {
        var disposables = new List<IDisposable>
        {
            LogContext.PushProperty("TestName", testName),
            LogContext.PushProperty("TestId", testId),
            LogContext.PushProperty("Browser", browser ?? string.Empty),
            LogContext.PushProperty("ThreadId", Thread.CurrentThread.ManagedThreadId)
        };

        return new CompositeDisposable(disposables);
    }

    // Record the expected outcome for the current test and log it.
    public static void SetExpected(string expected)
    {
        _expected.Value = expected;
        Logger.Information("=== EXPECTED: {Expected} ===", expected);
    }

    // Record/log the actual outcome for the current test. Includes previously recorded expected value if any.
    public static void SetActual(string actual)
    {
        var expected = _expected.Value ?? string.Empty;
        Logger.Information("=== ACTUAL: {Actual} ===", actual);
        // Clear expected after logging
        _expected.Value = null;
    }

    // Helper composite disposable to dispose multiple LogContext pushes as one scope
    private class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _items;
        private bool _disposed;

        public CompositeDisposable(List<IDisposable> items)
        {
            _items = items;
        }

        public void Dispose()
        {
            if (_disposed) return;
            // dispose in reverse order
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                try { _items[i].Dispose(); } catch { }
            }
            _disposed = true;
        }
    }
}