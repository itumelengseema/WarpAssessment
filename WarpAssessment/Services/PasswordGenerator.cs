using WarpAssessment.Interfaces;

namespace WarpAssessment.Services;

/// <summary>
/// Generates all permutations of the word "password" with character substitutions.
/// Supports: lowercase/uppercase, a->@, s->5, o->0
/// </summary>
public class PasswordGenerator : IPasswordGenerator
{
    private const string BaseWord = "password";

    /// <summary>
    /// Generates all unique password permutations and returns them as a materialized list.
    /// </summary>
    /// <returns>A list of unique password permutations.</returns>
    public IEnumerable<string> Generate()
    {
        var results = new HashSet<string>(); // Use HashSet to automatically eliminate duplicates
        var chars = BaseWord.ToCharArray();
        
        GenerateRecursive(chars, 0, results);
        
        // Materialize and sort for consistency
        return results.OrderBy(p => p).ToList();
    }

    /// <summary>
    /// Recursively generates all permutations by trying different character options at each position.
    /// </summary>
    private void GenerateRecursive(char[] chars, int index, HashSet<string> results)
    {
        if (index >= chars.Length)
        {
            results.Add(new string(chars));
            return;
        }

        var original = chars[index];
        var options = GetCharacterOptions(original);

        foreach (var option in options)
        {
            chars[index] = option;
            GenerateRecursive(chars, index + 1, results);
        }

        // Restore original character for backtracking
        chars[index] = original;
    }

    /// <summary>
    /// Gets all valid character options for a given character.
    /// Applies substitution rules: a->@, s->5, o->0, plus lowercase/uppercase variants.
    /// </summary>
    private static IEnumerable<char> GetCharacterOptions(char c)
    {
        var options = new HashSet<char>
        {
            char.ToLower(c),
            char.ToUpper(c)
        };

        var lowerChar = char.ToLower(c);
        
        // Apply substitution rules
        if (lowerChar == 'a') options.Add('@');
        if (lowerChar == 's') options.Add('5');
        if (lowerChar == 'o') options.Add('0');

        return options;
    }
}