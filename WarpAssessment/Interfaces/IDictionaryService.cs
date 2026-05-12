namespace WarpAssessment.Interfaces;

public interface IDictionaryService
{
    /// <summary>
    /// Writes passwords to a file asynchronously.
    /// </summary>
    Task WriteToFileAsync(IEnumerable<string> words, string path);

    /// <summary>
    /// Reads passwords from a file asynchronously.
    /// </summary>
    Task<IEnumerable<string>> ReadFromFileAsync(string path);
}