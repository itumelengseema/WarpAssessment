using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using WarpAssessment.Interfaces;
using WarpAssessment.Models;

namespace WarpAssessment.Services;

/// <summary>
/// Handles uploading the Base64 ZIP file to the temporary upload URL.
/// </summary>
public class UploadService : IUploadService
{
    private readonly HttpClient _httpClient;
    private readonly UploadSettings _settings;
    private readonly ILogger<UploadService> _logger;
    private DateTime _lastUploadTime = DateTime.MinValue;

    public UploadService(
        HttpClient httpClient,
        UploadSettings settings,
        ILogger<UploadService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Configure HttpClient timeout
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
    }

    /// <summary>
    /// Uploads the Base64 ZIP payload to the specified URL with rate limiting.
    /// </summary>
    /// <param name="url">The temporary upload URL.</param>
    /// <param name="base64Data">The Base64 encoded ZIP data.</param>
    public async Task UploadAsync(string url, string base64Data)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentNullException(nameof(url));
        if (string.IsNullOrWhiteSpace(base64Data))
            throw new ArgumentNullException(nameof(base64Data));

        try
        {
            // Enforce upload rate limit (5 minutes between uploads)
            var timeSinceLastUpload = DateTime.UtcNow - _lastUploadTime;
            if (timeSinceLastUpload < TimeSpan.FromSeconds(5))
            {
                var waitTime = TimeSpan.FromSeconds(5) - timeSinceLastUpload;
                _logger.LogInformation($"Rate limit: waiting {waitTime.TotalSeconds:F2} seconds before upload");
                await Task.Delay(waitTime);
            }

            var payload = new
            {
                data = base64Data,
                name = _settings.Name,
                surname = _settings.Surname,
                email = _settings.Email
            };

            var json = JsonSerializer.Serialize(payload);

            _logger.LogInformation($"Uploading payload to: {url}");
            _logger.LogInformation($"Payload size: {base64Data.Length / 1024 / 1024}MB (Base64)");

            var response = await _httpClient.PostAsync(
                url,
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            _lastUploadTime = DateTime.UtcNow;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                _logger.LogInformation($"Upload successful! Status: {response.StatusCode}");
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"Upload failed. Status: {response.StatusCode}");
                _logger.LogWarning($"Response: {content}");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"HTTP error during upload: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error during upload: {ex.Message}");
            throw;
        }
    }
}