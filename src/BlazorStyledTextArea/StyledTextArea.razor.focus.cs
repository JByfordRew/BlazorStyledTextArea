namespace BlazorStyledTextArea;

public partial class StyledTextArea 
{
    public async Task Focus()
    {
        await PauseTypeahead();
        await jsInterop!.FocusTextArea(TextArea);
        await UpdateTypeahead();
    }
}
