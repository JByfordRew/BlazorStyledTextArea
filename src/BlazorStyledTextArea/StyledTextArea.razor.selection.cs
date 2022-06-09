namespace BlazorStyledTextArea;

public partial class StyledTextArea 
{
    private bool isSelectingWithMouse = false;

    private void MouseSelectionStart() => isSelectingWithMouse = true;

    private async Task MouseSelecting()
    {
        if (isSelectingWithMouse)
        {
            await GetTextAreaCaret();
        }
    }
   
    private async Task MouseSelectionEnd()
    {
        if (isSelectingWithMouse)
        {
            isSelectingWithMouse = false;
            await GetTextAreaCaret();
        }
    }
}
