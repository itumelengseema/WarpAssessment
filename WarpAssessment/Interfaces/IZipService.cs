namespace WarpAssessment.Interfaces;

public interface IZipService
{
    string CreateBase64Zip(string[] filePaths);
}