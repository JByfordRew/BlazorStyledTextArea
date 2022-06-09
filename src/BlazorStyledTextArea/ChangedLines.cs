namespace BlazorStyledTextArea;

public class ChangedLines
{
    public ChangedLines(List<string> lines, int startIndex, int length, List<StyleRule> expiredStyleRules)
    {
        Lines = lines;
        StartIndex = startIndex;
        Length = length;
        ExpiredStyleRules = expiredStyleRules;
    }

    /// <summary>
    /// All lines.
    /// </summary>
    public List<string> Lines { get; }

    /// <summary>
    /// Index of first change.
    /// </summary>
    public int StartIndex { get; }

    /// <summary>
    /// Length of changes from StartIndex.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// StyleRules that have not been met and should be removed and recalculated.
    /// </summary>
    public List<StyleRule> ExpiredStyleRules { get; }
}
