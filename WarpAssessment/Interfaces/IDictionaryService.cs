namespace WarpAssessment.Interfaces;

public interface IDictionaryService
{
    Task WriteToFileAsync(IEnumerable<string> words,string path);
}