using Microsoft.AspNetCore.Components;

namespace BlazorStyledTextArea;

public partial class StyledTextArea 
{
    private async Task RefreshMarkup()
    {
        deferSetCaretPositionUntilRendered = true;
        deferredCaretPosition = caretPosition;
        await DeferredSetCaret();
    }

    private MarkupString GetMarkup(int i)
    {
        if (rowCol.Row != endRowCol.Row && i >= rowCol.Row && i <= endRowCol.Row)
        {
            if (i == rowCol.Row && i != endRowCol.Row)
            {
                return new MarkupString(markupLines[i].Markup.ToString().
                    RemoveMarkupBetween(rowCol.Col, markupLines[i].Text.Length));

            }
            else if (i == endRowCol.Row && i != rowCol.Row && endRowCol.Col > 0)
            {
                return new MarkupString(markupLines[i].Markup.ToString().
                    RemoveMarkupBetween(0, endRowCol.Col));
            }
            else if (i > rowCol.Row && i < endRowCol.Row)
            {
                return new MarkupString(markupLines[i].Text);
            }
        }
        else if (i == caretData.Row)
        {
            if (rowCol.Col == endRowCol.Col)
            {
                return editingLineMarkupString;
            }

            return new MarkupString(markupLines[i].Markup.ToString().
                        RemoveMarkupBetween(rowCol.Col, endRowCol.Col));
        }

        return markupLines[i].Markup;
    }
}
