using Microsoft.Extensions.Logging;
using WarpAssessment.Interfaces;

namespace WarpAssessment.Services;

/// <summary>
/// Handles reading and writing password dictionaries to files asynchronously.
/// </summary>
public class DictionaryService : IDictionaryService
{
    private readonly ILogger<DictionaryService> _logger;

    public DictionaryService(ILogger<DictionaryService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Writes all passwords to a file asynchronously, one per line.
    /// </summary>
    /// <param name="words">The collection of passwords to write.</param>
    /// <param name="path">The file path where passwords will be written.</param>
    /// <exception cref="ArgumentNullException">Thrown when path is null.</exception>
    /// <exception cref="IOException">Thrown when file write operations fail.</exception>
    public async Task WriteToFileAsync(IEnumerable<string> words, string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        try
        {
            // Materialize the enumerable to ensure we have all passwords
            var passwordList = words.ToList();
            _logger.LogInformation($"Writing {passwordList.Count} passwords to {path}");

            // Create parent directory if it doesn't exist
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                _logger.LogInformation($"Created directory: {directory}");
            }

            // Write passwords to file asynchronously
            await File.WriteAllLinesAsync(path, passwordList);
            _logger.LogInformation($"Successfully wrote {passwordList.Count} passwords to {path}");

            // Verify the file was created and has content
            if (!File.Exists(path))
                throw new IOException($"File was not created at {path}");

            var lineCount = (await File.ReadAllLinesAsync(path)).Length;
            if (lineCount == 0)
                throw new IOException($"File was created but contains no lines: {path}");

            _logger.LogInformation($"File verification successful. File contains {lineCount} lines.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to write passwords to file: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Reads all passwords from a file asynchronously.
    /// </summary>
    /// <param name="path">The file path to read from.</param>
    /// <returns>A collection of passwords read from the file.</returns>
    public async Task<IEnumerable<string>> ReadFromFileAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        try
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Dictionary file not found: {path}");

            var lines = await File.ReadAllLinesAsync(path);
            _logger.LogInformation($"Read {lines.Length} passwords from {path}");
            return lines;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to read passwords from file: {ex.Message}");
            throw;
        }
    }
}