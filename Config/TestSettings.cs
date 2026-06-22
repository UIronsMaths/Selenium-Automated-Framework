public class TestSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public bool Headless { get; set; } = false;
    public int ExplicitWaitSeconds { get; set; } = 2;
    public int PageLoadTimeoutSeconds { get; set; } = 30;
    public string ReportType { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string BlankUsername {  get; set; }  = string.Empty;
    public string LockedUser {  get; set; } = string.Empty;
    public string InvalidUser {  get; set; } = string.Empty;
    public string SpacedUser { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string BlankPassword {  get; set; } = string.Empty;
    public string WrongPassword {  get; set; } = string.Empty;
    public string SpacedPassword {  get; set; } = string.Empty;
    public string ValidBuyerFN { get; set; } = string.Empty;
    public string ValidBuyerLN { get; set; } = string.Empty;
    public string ValidZipCode { get; set; } = string.Empty;
    public string ScreenshotDirectory { get; set; } = string.Empty;
    public string ExtentReportDirectory { get; set; } = string.Empty;
    public string LogDirectory { get; set; } = string.Empty;
    public string AllureDirectory { get; set; } = string.Empty;
    public string BaseDirectory { get; set;  } = string.Empty;
}