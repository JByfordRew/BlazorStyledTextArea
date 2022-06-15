using Microsoft.AspNetCore.Components;
using System.Text.Encodings.Web;

namespace BlazorStyledTextArea;

public class MarkupLine
{
    public const string CursorElementId = $"{nameof(BlazorStyledTextArea)}Caret";
    public const string CursorPlaceholder = $"<span class='caret' style='visibility: hidden;' id='{CursorElementId}'><i></i></span>";
    public const string TypedWordElementId = $"{nameof(BlazorStyledTextArea)}Word";
    public const string TypedWordOpeningTag = $"<span style='white-space: nowrap; display: inline-block;' id='{TypedWordElementId}'>";

    public Guid Id { get; private set; }
    public string Text { get; private set; }
    public List<MarkupLine> LinesList {  get; }
    public List<StyleRule> StyleRules { get; private set; }
    public MarkupString Markup { get; private set; }

    private MarkupLine(string text, List<MarkupLine> linesList, List<StyleRule> styleRules, MarkupString markup)
    {
        Id = Guid.NewGuid();
        Text = text;
        LinesList = linesList;
        StyleRules = styleRules;
        Markup = markup;
    }

    public static MarkupLine Placeholder(List<MarkupLine> linesList)
        => new(Guid.NewGuid().ToString(), linesList, new List<StyleRule>(), new MarkupString(""));

    public async Task<List<StyleRule>> Update(string text, List<StyleRule> styleRules)
    {
        this.Id = Guid.NewGuid();
        this.Text = text;
        this.StyleRules = styleRules;
        var html = await Text.ApplyMatchedStyles(StyleRules, LinesList.IndexOf(this));
        this.Markup = new MarkupString(html);
        styleRules.ForEach(x => x.IsNew = false);
        return styleRules.Where(x=> !x.IsMatched).ToList();
    }

    public async Task<MarkupString> UpdateMarkupWithHiddenCursorElementAt(
        int col, string typeaheadSuggestion, string wordToCaret) 
    {
        wordToCaret = typeaheadSuggestion.Any() ? wordToCaret : "";

        typeaheadSuggestion = HtmlEncoder.Default.Encode(typeaheadSuggestion);

        var html = this.Markup.Value;

        var wordStartCol = col - wordToCaret.Length;

        var placeholder = CursorPlaceholder.Replace("</span>", $"{typeaheadSuggestion}</span>");

        if (wordToCaret.Any())
        {
            wordStartCol = html.IndexOfTextWithMarkup(wordStartCol);
            html = html.Insert(wordStartCol, TypedWordOpeningTag);
            placeholder += "</span>";
        }
        else
        {
            placeholder = placeholder.Replace("class='caret'", "");
        }

        col = html.IndexOfTextWithMarkup(col);
        html = html.Insert(Math.Min(Math.Max(0, col), html.Length), placeholder);

        return await Task.FromResult(new MarkupString(html));
    }
}
