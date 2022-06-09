using System.Net;

namespace BlazorStyledTextArea;

public class StyleRule
{
    public const string MatchMarker = "{match}";

    private readonly Dictionary<string, string> abbreviatedStyles = new()
    {
        { "bold", "b" },
        { "italic", "i" },
        { "replace", "r" },
        { "rounded", "o" },
        { "underline", "u" },
        { "strikethrough", "s" },
        { "clickable", "x" }
    };

    private StyleRule(
        string matchText,
        string styleClassNames,
        string? html,
        int? lineNumber,
        int? matchInstance,
        bool wordsOnly,
        string? replaceWith,
        string attributes,
        string? name,
        string? id,
        bool startOfLine)
    {

        MatchText = matchText;
        StyleClassNames = styleClassNames.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        WrapHtml = html;
        LineIndex = lineNumber;
        MatchInstance = matchInstance;
        WordsOnly = wordsOnly;
        Replace = replaceWith;
        Attributes = attributes;
        Name = name;
        Id = id;
        StartOfLine = startOfLine;
        IsMatched = true;
        IsNew = true;

        AbbreviateStyleClassNames();
    }

    private void AbbreviateStyleClassNames()
    {
        foreach (var abbreviation in abbreviatedStyles)
        {
            if (StyleClassNames.Contains(abbreviation.Key))
            {
                StyleClassNames[StyleClassNames.FindIndex(x => x.Equals(abbreviation.Key))] = abbreviation.Value;
            }
        }
    }

    /// <summary>
    /// Specify text to match and css class names to apply.
    /// </summary>
    /// <param name="text">Text to match</param>
    /// <param name="styleClassNames">Any css class names.  Inbuilt class names include; bold, italic, rounded, replace, underline, strikethrough.</param>
    /// <returns>StyleRule to continue configuring</returns>
    public static StyleRule Text(string text, string styleClassNames)
        => new(WebUtility.HtmlEncode(text), styleClassNames, null, null, null, false, null, "", null, null, false);

    /// <summary>
    /// Specify words to match and css class names to apply.
    /// </summary>
    /// <param name="text">Text to match when they are words</param>
    /// <param name="styleClassNames">Any css class names.  Predefined class names include; bold, italic, rounded, replace, underline, strikethrough.</param>
    /// <returns>StyleRule to continue configuring</returns>
    public static StyleRule Words(string text, string styleClassNames)
        => new(WebUtility.HtmlEncode(text.Trim()), styleClassNames, null, null, null, true, null, "", null, null, false);

    /// <summary>
    /// Classify this style rule for your own needs. Useful for managing expired style rules
    /// </summary>
    /// <param name="name">Name of the classification of this style rule</param>
    /// <returns>StyleRule to continue configuring</returns>
    public StyleRule Named(string name)
        => new(MatchText, StyleClassNamesString, WrapHtml, LineIndex, MatchInstance, WordsOnly, Replace, Attributes, name, Id, StartOfLine);

    /// <summary>
    /// Makes styled text clickable.  Events will be raised via callback OnElementClicked with the id and matched text.
    /// </summary>
    /// <param name="id">Any text string for your Id</param>
    /// <returns>StyleRule to continue configuring</returns>
    public StyleRule WithId(string id)
        => new(MatchText, StyleClassNamesString, WrapHtml, LineIndex, MatchInstance, WordsOnly, Replace, Attributes, Name, id, StartOfLine);

    /// <summary>
    /// Style rule will only be matched on this line of the text
    /// </summary>
    /// <param name="lineIndex">Index of the line (zero based)</param>
    /// <returns>StyleRule to continue configuring</returns>
    public StyleRule OnLine(int lineIndex)
        => new(MatchText, StyleClassNamesString, WrapHtml, lineIndex, MatchInstance, WordsOnly, Replace, Attributes, Name, Id, StartOfLine);

    /// <summary>
    /// Only applies style rule on the matched instance in the line.
    /// </summary>
    /// <param name="matchInstance">Index of match (zero based)</param>
    /// <returns>StyleRule to continue configuring</returns>
    public StyleRule AtInstance(int matchInstance)
        => new(MatchText, StyleClassNamesString, WrapHtml, LineIndex, matchInstance, WordsOnly, Replace, Attributes, Name, Id, StartOfLine);

    /// <summary>
    /// Replaces matched text with HTML.  Use StyleRule.MatchMarker to inject the matched text.
    /// </summary>
    /// <param name="html">Any valid HTML (Blazor components not currently supported)</param>
    /// <returns>StyleRule to continue configuring</returns>
    public StyleRule HtmlTemplate(string html)
        => new(MatchText, StyleClassNamesString, html, MatchInstance, LineIndex, WordsOnly, Replace, Attributes, Name, Id, StartOfLine);

    /// <summary>
    /// On apply style rule if match is at start of a line (including whitespace)
    /// </summary>
    /// <returns>StyleRule to continue configuring</returns>
    public StyleRule AtStartOfLine()
        => new(MatchText, StyleClassNamesString, WrapHtml, LineIndex, MatchInstance, WordsOnly, Replace, Attributes, Name, Id, true);

    /// <summary>
    /// Replace matched text with alternative text. Good for bullet point styling.
    /// </summary>
    /// <param name="withText">Text to replace matched text</param>
    /// <returns>StyleRule to continue configuring</returns>
    public StyleRule ReplaceWith(string withText)
    {
        withText = WebUtility.HtmlEncode(withText);

        if (!StyleClassNames.Contains("r"))
        {
            StyleClassNames.Add("r");
        }
        var attributesToAdd = Attributes + $" data-v='{withText}'";

        return new StyleRule(MatchText, StyleClassNamesString, WrapHtml, LineIndex, MatchInstance, WordsOnly, withText, attributesToAdd, Name, Id, StartOfLine);
    }

    private string StyleClassNamesString => String.Join(' ', StyleClassNames);

    internal string MatchText { get; }
    internal int? MatchInstance { get; }
    internal List<string> StyleClassNames { get; }
    internal string? WrapHtml { get; }
    internal bool WordsOnly { get; }
    internal string? Replace { get; }
    internal bool IgnoreCase { get; }
    internal string Attributes { get; }
    internal bool StartOfLine { get; }
    internal bool IsMatched { get; set; }
    internal bool IsNew { get; set; }

    /// <summary>
    /// Name used to categorise this style rule via Named method
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Id as specified by WithId method.
    /// </summary>
    public string? Id { get; }

    /// <summary>
    /// Line index of text lines.  This may change automatically when editing occurs.
    /// </summary>
    public int? LineIndex { get; internal set; }

}
