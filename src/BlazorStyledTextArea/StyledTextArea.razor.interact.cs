using Microsoft.JSInterop;

namespace BlazorStyledTextArea;

public partial class StyledTextArea 
{
    [JSInvokable]
    public void MarkupClicked(string id, string text)
    {
        OnElementClicked.InvokeAsync(new Element(id, text));
    }

    [JSInvokable]
    public void MarkupMouseMoved(string id)
    {
        whenLastClickableMouseMoved = DateTimeOffset.UtcNow;
    }

    private DateTimeOffset whenLastClickableMouseMoved = default;
    private bool isProcessingClickableMouseMoved = false;

    private async Task AllowClickableMarkupWhenEditing()
    {
        if (StyleRules.Any(_ => _.Id is not null) && isEditing && !isProcessingClickableMouseMoved)
        {
            isProcessingClickableMouseMoved = true;

            isEditing = false;
           
            await Task.Delay(100);

            var timeSinceLastMouseMoved = DateTimeOffset.UtcNow - whenLastClickableMouseMoved;

            if (timeSinceLastMouseMoved.TotalMilliseconds > TimeElementIsClickableInMilliseconds)
            {
                isEditing = true;
            }
            else
            {
                await Task.Delay(TimeElementIsClickableInMilliseconds);
                isEditing = true;
            }

            isProcessingClickableMouseMoved = false;
        }
    }
}
