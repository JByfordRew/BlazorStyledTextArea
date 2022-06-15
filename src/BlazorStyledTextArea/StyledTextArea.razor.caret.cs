namespace BlazorStyledTextArea;

public partial class StyledTextArea 
{
    private int caretPosition = 0;
    private int textLength = 0;
    private int deferredCaretPosition = 0;
    private Coords caretCoords = new(0, 0);
    private Coords wordStartCoords = new(0, 0);
    private RowCol rowCol = new(0, 0);
    private RowCol endRowCol = new(0, 0);
    private bool deferSetCaretPositionUntilRendered = false;
    private bool hasSelection => rowCol.Row != endRowCol.Row || rowCol.Col != endRowCol.Col;

    private async Task DeferredSetCaret()
    {
        if (deferSetCaretPositionUntilRendered)
        {
            deferSetCaretPositionUntilRendered = false;
            await jsInterop!.SetTextAreaCaret(TextArea, deferredCaretPosition);
            await GetTextAreaCaret(); //NOTE if this is not done will not clear typeahead!
            await jsInterop!.SetTextAreaCaret(TextArea, deferredCaretPosition);
        }
    }

    private async Task GetTextAreaCaret()
    {
        var values = await jsInterop!.GetTextAreaCaret(TextArea);
        if (values is not null)
        {
            caretPosition = values[0];
            rowCol = new RowCol(values[1], values[2]);
            endRowCol = new RowCol(values[3], values[4]);
            textLength = values[5];
            await UpdateCaretData(); //NOTE if this is not done typeahead will be in the wrong place and stops the jerkyness of typeahead experience as user types
        }
    }

    private async Task UpdateCursorPosition()
    {
        var coords = await jsInterop!.GetElementPosition(this.id.ToString(), MarkupLine.CursorElementId);
        caretCoords = new Coords(coords[0], coords[1]);
        await Task.Yield(); //NOTE important to update markup with typeahead content
        coords = await jsInterop!.GetElementPosition(this.id.ToString(), MarkupLine.TypedWordElementId);
        wordStartCoords = new Coords(coords[0], coords[1]);
    }

    private async Task UpdateCaretData()
    {
        var value = new CaretData(
        caretPosition, textLength,
        rowCol.Row, rowCol.Col,
        caretCoords.Y, caretCoords.X,
        text.IsAtEndOfLine(caretPosition), text.LastWordTextToCaret(caretPosition),
        wordStartCoords.Y, wordStartCoords.X);

        if (!caretData.Equals(value))
        {
            caretData = value;
            await UpdateMarkupCurrentLine();
            await OnCaretChanged.InvokeAsync(value);
        }
    }
}
