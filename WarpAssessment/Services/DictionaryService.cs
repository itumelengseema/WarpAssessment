using WarpAssessment.Interfaces;

namespace WarpAssessment.Services;

public class DictionaryService: IDictionaryService
{
    public async Task WriteToFileAsync(IEnumerable<string> words, string path)
    {
        await File.WriteAllLinesAsync(path, words);
    
    }
}