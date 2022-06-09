namespace BlazorStyledTextArea;

public partial class StyledTextArea 
{
    /// <summary>
    /// Inserts text after the word currently typed to the caret position.
    /// Caret position is advanced to end of the inserted text
    /// Textarea is given focus
    /// </summary>
    /// <param name="value">Text to be inserted</param>
    /// <returns></returns>
    public async Task InsertText(string value) => await AlterText(value, "");

    /// <summary>
    /// Replaces the word currently typed to the caret position with the replacement text.
    /// Caret position is advanced to end of replacement text
    /// Textarea is given focus
    /// </summary>
    /// <param name="value">Replacement text</param>
    /// <returns></returns>
    public async Task ReplaceWord(string value) => await AlterText(value, caretData.WordTextToCaret);

    private async Task AlterText(string value, string word)
    {
        await Focus();
        await Task.Yield(); //NOTE stops janky typeahead experience when inserting text
        deferSetCaretPositionUntilRendered = true;
        deferredCaretPosition = caretPosition - word.Length + value.Length;
        Text = Text.Remove(caretPosition - word.Length, word.Length)
                    .Insert(caretPosition - word.Length, value);
        await UpdateTypeahead();
    }
}
