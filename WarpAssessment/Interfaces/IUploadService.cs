namespace WarpAssessment.Interfaces;

public interface IUploadService
{
    Task UploadAsync(string url, string base64Data);
}