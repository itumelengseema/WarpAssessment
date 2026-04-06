using System.Net;
using System.Net.Http.Headers;
using System.Text;
using WarpAssessment.Interfaces;

namespace WarpAssessment.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> AuthenticateAsync(string username, string password)
    {
        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{username}:{password}"));

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://recruitment.warpdevelopment.co.za/v2/api/authenticate"
        );
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        
        var response = await _httpClient.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            return  await response.Content.ReadAsStringAsync();
        }
        return null;
    }
}