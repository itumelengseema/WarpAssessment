namespace WarpAssessment.Interfaces;

public interface IPasswordGenerator
{
    IEnumerable<string> Generate();
}