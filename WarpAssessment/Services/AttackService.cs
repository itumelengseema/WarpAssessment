using WarpAssessment.Interfaces;

namespace WarpAssessment.Services;

public class AttackService
{
    private readonly IAuthService _authService;

    public AttackService(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<string?> ExecuteAsync(IEnumerable<string> passwords)
    {
        int attempt = 0;

        foreach (var password in passwords)
        {
            attempt++;
            Console.WriteLine($"[{attempt} Trying: {password}]");
            
            var result = await _authService.AuthenticateAsync("John", password);

            if (result != null)
            {
                Console.WriteLine($"SUCCESS: {password}");
                return result;
            }
            await Task.Delay(100);
        }
        return null;
    }
}