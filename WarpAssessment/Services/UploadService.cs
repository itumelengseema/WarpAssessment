using System.Text;
using System.Text.Json;
using WarpAssessment.Interfaces;

namespace WarpAssessment.Services;

public class UploadService : IUploadService
{
    private readonly HttpClient _httpClient;
    public UploadService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task UploadAsync(string url, string base64Data)
    {
        var payload = new
        {
            data = base64Data,
            name = "Itumeleng",
            surname = "Seema",
            email = "itumelengseema@outlook.com"
        };
        
        var json = JsonSerializer.Serialize(payload);
        
        var response = await _httpClient.PostAsync(
            url,
            new StringContent(json,Encoding.UTF8,"application/json")
            
            );
        
        Console.WriteLine($"Upload Status: {response.StatusCode}");
        
        
        
    }
}