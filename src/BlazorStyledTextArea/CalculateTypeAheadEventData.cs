namespace BlazorStyledTextArea;

public class CalculateTypeaheadEventData
{
    public CalculateTypeaheadEventData(
        int textIndex,
        bool atEndOfLine,
        string wordTextToCaret,
        char nextChar,
        bool recommendedPauseAndReturnEmptyString)
    {
        TextIndex = textIndex;
        AtEndOfLine = atEndOfLine;
        WordTextToCaret = wordTextToCaret;
        NextChar = nextChar;
        RecommendPauseAndClearTypeahead = recommendedPauseAndReturnEmptyString;
    }

    /// <summary>
    /// Index in text of the caret position
    /// </summary>
    public int TextIndex { get; }

    /// <summary>
    /// True, if remaining line text after the caret is whitespace or none
    /// </summary>
    public bool AtEndOfLine { get; }

    /// <summary>
    /// The text of the last word leading up to the caret position. 
    /// Use to assist in creating typeahead suggestion(s)
    /// </summary>
    public string WordTextToCaret { get; }

    /// <summary>
    /// The character to the right of the caret
    /// </summary>
    public char NextChar { get; }

    /// <summary>
    /// The component has recommended to pause calculation of the typeahead suggestion and return an empty string instead.
    /// This is based on default user behaviour including;
    /// - Pressing Escape to cancel typeahead suggestion
    /// - Pressing Delete to cancel typeahead suggestion
    /// </summary>
    public bool RecommendPauseAndClearTypeahead { get; }

    /// <summary>
    /// Recommendation on showing typeahead based on; 
    /// A word has been started to be typed, 
    /// The word is the start of the suggestion ignoring case,
    /// The word is not the suggestion ignoring case,
    /// The next character is not a letter or digit or at the end of line
    /// </summary>
    public bool ShouldShowTypeahead(string typeahead) =>
        WordTextToCaret.Length > 0 
        && typeahead.StartsWith(WordTextToCaret, StringComparison.InvariantCultureIgnoreCase)
        && !typeahead.Equals(WordTextToCaret, StringComparison.InvariantCultureIgnoreCase)
        && (!char.IsLetterOrDigit(NextChar) || AtEndOfLine);

    /// <summary>
    /// Recommends the typeahead suggestion using this ShouldShowTypeahead
    /// and returning the remainder of the typeahead value
    /// </summary>
    /// <param name="typeahead"></param>
    /// <returns></returns>
    public string DefaultSuggestion(string typeahead) =>
        ShouldShowTypeahead(typeahead) ?
            typeahead[Math.Min(typeahead.Length, WordTextToCaret.Length)..]
            : "";
}
