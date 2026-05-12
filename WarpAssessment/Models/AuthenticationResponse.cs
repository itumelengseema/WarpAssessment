using System.Text.Json.Serialization;

namespace WarpAssessment.Models;

/// <summary>
/// Response from the authentication endpoint.
/// </summary>
public class AuthenticationResponse
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
