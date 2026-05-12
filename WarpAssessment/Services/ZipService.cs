using System.IO.Compression;
using Microsoft.Extensions.Logging;
using WarpAssessment.Interfaces;

namespace WarpAssessment.Services;

/// <summary>
/// Handles creation and conversion of ZIP files to Base64 format.
/// </summary>
public class ZipService : IZipService
{
    private readonly ILogger<ZipService> _logger;

    public ZipService(ILogger<ZipService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a ZIP file from the specified files and converts it to Base64.
    /// </summary>
    /// <param name="filePaths">Array of file paths to include in the ZIP.</param>
    /// <returns>Base64 encoded string of the ZIP file.</returns>
    /// <exception cref="ArgumentNullException">Thrown when filePaths is null.</exception>
    /// <exception cref="FileNotFoundException">Thrown when a required file doesn't exist.</exception>
    public string CreateBase64Zip(string[] filePaths)
    {
        if (filePaths == null)
            throw new ArgumentNullException(nameof(filePaths));

        _logger.LogInformation($"Creating ZIP with {filePaths.Length} files");

        using var memoryStream = new MemoryStream();

        try
        {
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (var filePath in filePaths)
                {
                    if (string.IsNullOrWhiteSpace(filePath))
                    {
                        _logger.LogWarning("Skipping null or empty file path");
                        continue;
                    }

                    var fileName = Path.GetFileName(filePath);
                    var exists = File.Exists(filePath);

                    _logger.LogInformation($"Processing file: {fileName} (Exists: {exists})");

                    if (!exists)
                    {
                        _logger.LogWarning($"File not found, skipping: {filePath}");
                        continue;
                    }

                    try
                    {
                        var entry = archive.CreateEntry(fileName);
                        using (var entryStream = entry.Open())
                        using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            fileStream.CopyTo(entryStream);
                        }

                        _logger.LogInformation($"Successfully added to ZIP: {fileName}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to add file to ZIP: {fileName} - {ex.Message}");
                        throw;
                    }
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            var base64 = Convert.ToBase64String(memoryStream.ToArray());
            _logger.LogInformation($"ZIP created successfully. Base64 length: {base64.Length} characters");
            
            return base64;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to create ZIP file: {ex.Message}");
            throw;
        }
    }
}