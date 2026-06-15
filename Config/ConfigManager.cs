using Microsoft.Extensions.Configuration;

public static class ConfigurationManager
{
    private static readonly Lazy<TestSettings> _settings = new Lazy<TestSettings>(Load);

    public static TestSettings Settings => _settings.Value;

    private static TestSettings Load()
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var settings = new TestSettings();
        config.GetSection("TestSettings").Bind(settings);

        // Apply env-var overrides using the SAUCE_ prefix convention
        // shown in the document's PowerShell examples, e.g. $env:SAUCE_BROWSER="firefox"
        ApplyOverrides(settings);

        return settings;
    }

    private static void ApplyOverrides(TestSettings settings)
    {
        var browser = Environment.GetEnvironmentVariable("SAUCE_BROWSER");
        if (!string.IsNullOrWhiteSpace(browser))
            settings.Browser = browser;

        var headless = Environment.GetEnvironmentVariable("SAUCE_HEADLESS");
        if (bool.TryParse(headless, out var headlessBool))
            settings.Headless = headlessBool;

        var reportType = Environment.GetEnvironmentVariable("SAUCE_REPORT_TYPE");
        if (!string.IsNullOrWhiteSpace(reportType))
            settings.ReportType = reportType;

        var password = Environment.GetEnvironmentVariable("SAUCE_PASSWORD");
        if (!string.IsNullOrWhiteSpace(password))
            settings.Password = password;

        var username = Environment.GetEnvironmentVariable("SAUCE_USERNAME");
        if (!string.IsNullOrWhiteSpace(username))
            settings.Username = username;

        var baseUrl = Environment.GetEnvironmentVariable("SAUCE_BASE_URL");
        if (!string.IsNullOrWhiteSpace(baseUrl))
            settings.BaseUrl = baseUrl;
    }
}