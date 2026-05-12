namespace WarpAssessment.Interfaces;

public interface IPasswordGenerator
{
    /// <summary>
    /// Generates all unique permutations of the password with character substitutions.
    /// </summary>
    /// <returns>A collection of generated passwords.</returns>
    IEnumerable<string> Generate();
}