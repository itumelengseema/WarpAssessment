using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using WarpAssessment.Helpers;
using WarpAssessment.Interfaces;
using WarpAssessment.Models;

namespace WarpAssessment.Services;

/// <summary>
/// Handles authentication against the Warp Development API.
/// </summary>
public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthSettings _settings;
    private readonly RateLimiter _rateLimiter;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        HttpClient httpClient,
        AuthSettings settings,
        RateLimiter rateLimiter,
        ILogger<AuthService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Configure HttpClient timeout
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
    }

    /// <summary>
    /// Authenticates with the API using Basic Authentication.
    /// Returns the URL from successful authentication or null if unsuccessful.
    /// </summary>
    public async Task<string?> AuthenticateAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentNullException(nameof(username));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException(nameof(password));

        try
        {
            // Apply rate limiting before making request
            await _rateLimiter.WaitIfNeededAsync();

            var credentials = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{username}:{password}"));

            var url = $"{_settings.ApiBaseUrl}{_settings.AuthenticateEndpoint}";
            
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            _logger.LogInformation($"Attempting authentication with password: {password}");
            
            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Authentication successful with password: {password}");
                
                // Parse the URL from the response
                try
                {
                    var jsonDoc = JsonDocument.Parse(content);
                    var uploadUrl = jsonDoc.RootElement.GetProperty("url").GetString();
                    return uploadUrl;
                }
                catch (JsonException)
                {
                    // If response is just the URL directly
                    return content;
                }
            }
            else
            {
                _logger.LogDebug($"Authentication failed with password: {password} - Status: {response.StatusCode}");
            }

            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"HTTP error during authentication: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error during authentication: {ex.Message}");
            throw;
        }
    }
}