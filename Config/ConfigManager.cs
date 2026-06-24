using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class ConfigurationManager
{
    private static readonly Lazy<TestSettings> _settings = new Lazy<TestSettings>(Load);

    public static TestSettings Settings => _settings.Value;

    private static TestSettings Load()
    {
        // We now load configuration from a .env file only (no JSON back-compat).
        var envPath = FindEnvFile();
        if (string.IsNullOrEmpty(envPath))
            throw new FileNotFoundException("Required .env file not found. Place a .env file in the repository root or a parent folder.");

        var envDict = ParseDotEnv(envPath);

        // Map .env keys into configuration under TestSettings. Accept either SAUCE_ prefix or plain keys.
        var mapped = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in envDict)
        {
            var key = kv.Key?.Trim() ?? string.Empty;
            var value = kv.Value ?? string.Empty;
            if (string.IsNullOrEmpty(key))
                continue;

            string configKey;
            if (key.StartsWith("SAUCE_", System.StringComparison.OrdinalIgnoreCase))
            {
                var prop = key.Substring("SAUCE_".Length);
                configKey = $"TestSettings:{prop}";
            }
            else if (key.Contains("__"))
            {
                // support TestSettings__Property style
                configKey = key.Replace("__", ":");
            }
            else
            {
                configKey = $"TestSettings:{key}";
            }

            mapped[configKey] = value;
        }

        var builder = new ConfigurationBuilder()
            .AddInMemoryCollection(mapped)
            .AddEnvironmentVariables();

        IConfiguration config = builder.Build();

        var settings = new TestSettings();
        config.GetSection("TestSettings").Bind(settings);

        // Also apply SAUCE_ environment variable overrides if present
        ApplyOverrides(settings);

        return settings;
    }

    private static string? FindEnvFile()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, ".env");
            if (File.Exists(candidate))
                return candidate;

            dir = dir.Parent;
        }

        return null;
    }

    private static IDictionary<string, string> ParseDotEnv(string path)
    {
        var dict = new Dictionary<string, string>();
        foreach (var raw in File.ReadAllLines(path))
        {
            var line = raw.Trim();
            if (string.IsNullOrEmpty(line))
                continue;
            if (line.StartsWith("#") || line.StartsWith("//"))
                continue;

            var content = line;
            // support `export KEY=VALUE`
            if (content.StartsWith("export ", System.StringComparison.OrdinalIgnoreCase))
                content = content.Substring("export ".Length);

            var idx = content.IndexOf('=');
            if (idx <= 0)
                continue;

            var key = content.Substring(0, idx).Trim();
            var val = content.Substring(idx + 1).Trim();

            // strip surrounding quotes
            if ((val.StartsWith("\"") && val.EndsWith("\"")) || (val.StartsWith("'") && val.EndsWith("'")))
                val = val.Substring(1, val.Length - 2);

            dict[key] = val;
        }

        return dict;
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