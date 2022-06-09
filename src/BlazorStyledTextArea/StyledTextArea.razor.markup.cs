using Microsoft.AspNetCore.Components;

namespace BlazorStyledTextArea;

public partial class StyledTextArea
{
    private readonly List<MarkupLine> markupLines = new();

    private MarkupString editingLineMarkupString = new("");

    private async Task UpdateMarkupFromText(string text)
    {
        if (StyleRules is not null)
        {
            var changes = await markupLines.UpdateLines(text, StyleRules);
            await UpdateMarkupCurrentLine(); //NOTE need here as improves UX whilst typing
            changes = await AdjustForLineSplitOrJoin(changes);
            await OnLinesChanged.InvokeAsync(changes);
            await markupLines.UpdateLinesAffectedByNewlyAddedStyleRules(StyleRules);
            await UpdateMarkupCurrentLine(); //NOTE needed here to stop flicker on typing in markup
        }
    }

    private async Task<ChangedLines> AdjustForLineSplitOrJoin(ChangedLines changes)
    {
        if (changes.Length == 1)
        {
            var oldLineCount = oldText!.Replace("\n", "").Length;
            var newLineCount = text.Replace("\n", "").Length;
            var diff = newLineCount - oldLineCount;
            
            var start = Math.Max(0, changes.StartIndex - 1);

            if (Math.Abs(diff) == 1)
            {
                changes = new ChangedLines(changes.Lines, start, changes.Length + 1, changes.ExpiredStyleRules);
            }
        }

        return await Task.FromResult(changes);
    }

    private async Task UpdateMarkupCurrentLine() => editingLineMarkupString =
        await (markupLines[Math.Min(markupLines.Count - 1, rowCol.Row)]).UpdateMarkupWithHiddenCursorElementAt(
            rowCol.Col, typeaheadSuggestion, caretData.WordTextToCaret);
}
