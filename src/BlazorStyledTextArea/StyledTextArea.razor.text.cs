namespace BlazorStyledTextArea;

public partial class StyledTextArea 
{
    private string? oldText;

    private async Task UpdateText(string value)
    {
        if (!text.Equals(value))
        {
            oldText = text;
            text = value;
            
            if (typeaheadSuggestion.Any())
            {
                await Task.Yield(); //NOTE stops janky typeahead experience (but would delayd typed chars from appearing)
            }
            
            if (eventCallbackName == "oninput")
            {
                _value = text;
                await eventCallback.InvokeAsync(text);
            }

            await UpdateMarkupFromText(text);
        }
    }
}
