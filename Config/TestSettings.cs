public class TestSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public string Browser { get; set; } = "chrome";
    public bool Headless { get; set; } = false;
    public int ExplicitWaitSeconds { get; set; } = 10;
    public int PageLoadTimeoutSeconds { get; set; } = 30;
    public string ReportType { get; set; } = "extent";
    public string Username { get; set; } = string.Empty;
    public string BlankUsername {  get; set; }  = string.Empty;
    public string LockedUser {  get; set; } = string.Empty;
    public string InvalidUser {  get; set; } = string.Empty;
    public string SpacedUser { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string BlankPassword {  get; set; } = string.Empty;
    public string WrongPassword {  get; set; } = string.Empty;
    public string SpacedPassword {  get; set; } = string.Empty;
    public string ScreenshotDirectory { get; set; } = "artifacts/screenshots";
    public string ExtentReportDirectory { get; set; } = "artifacts/extent";
    public string LogDirectory { get; set; } = "artifacts/logs";
}