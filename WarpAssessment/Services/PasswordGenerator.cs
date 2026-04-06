using WarpAssessment.Interfaces;

namespace WarpAssessment.Services;

public class PasswordGenerator: IPasswordGenerator
{
    public IEnumerable<string> Generate()
    {
        var results = new List<string>();
        
        GenerateRecursive("password".ToCharArray(),0, results);
        return results;
    }

    private void GenerateRecursive(char[] chars, int index, List<string> results)
    {
       
        if (index >= chars.Length)
        {
            results.Add(new string(chars));
            return;
        }

        var original = chars[index]; // store original

        var options = GetOptions(original);

        foreach (var option in options)
        {
            chars[index] = option;
            GenerateRecursive(chars, index + 1, results);
        }


        chars[index] = original;
    }

    private IEnumerable<char> GetOptions(char c)
    {
        var list = new List<char>
        {
            char.ToLower(c),
            char.ToUpper(c)

        };
        
        if(char.ToLower(c) == 'a') list.Add('@');
        if(char.ToLower(c) == 's')  list.Add('5');
        if(char.ToLower(c) == 'o') list.Add('0');
        
        return list.Distinct();
    }
}