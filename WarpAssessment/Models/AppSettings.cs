namespace WarpAssessment.Models;

/// <summary>
/// Configuration settings for the application.
/// </summary>
public class AppSettings
{
    public AuthSettings Auth { get; set; } = new();
    public RateLimitSettings RateLimit { get; set; } = new();
    public UploadSettings Upload { get; set; } = new();
    public FileSettings Files { get; set; } = new();
}

public class AuthSettings
{
    public string ApiBaseUrl { get; set; } = "https://recruitment.warpdevelopment.co.za/v2/api";
    public string AuthenticateEndpoint { get; set; } = "/authenticate";
    public string Username { get; set; } = "John";
    public int TimeoutSeconds { get; set; } = 30;
}

public class RateLimitSettings
{
    /// <summary>
    /// Maximum requests per second for authentication endpoint.
    /// </summary>
    public int AuthenticateRequestsPerSecond { get; set; } = 10;

    /// <summary>
    /// Minimum delay in milliseconds between uploads.
    /// </summary>
    public int MinUploadDelayMilliseconds { get; set; } = 5000;
}

public class UploadSettings
{
    public string? Name { get; set; } = "Itumeleng";
    public string? Surname { get; set; } = "Seema";
    public string? Email { get; set; } = "itumelengseema@outlook.com";
    public int TimeoutSeconds { get; set; } = 60;
}

public class FileSettings
{
    public string DictionaryFileName { get; set; } = "dict.txt";
    public string CvFileName { get; set; } = "cv.pdf";
    public string ZipFileName { get; set; } = "submission.zip";
}
