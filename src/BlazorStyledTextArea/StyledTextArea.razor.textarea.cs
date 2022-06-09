using BlazorTextarea;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorStyledTextArea;

public partial class StyledTextArea 
{
    private Textarea textarea { get; set; } = null!;

    private ElementReference standardTextarea { get; set; }

    private ElementReference TextArea => textarea?.TextArea ?? standardTextarea;

    private bool isEditing = true;

    private async Task OnFocus(FocusEventArgs args) => isEditing = true;

    private async Task OnBlur(FocusEventArgs args)
    {
        if (!isCustomTypeaheadSuggestion || !lastKeyWasTypeaheadReservedKey)
        {
            await Task.Delay(300); //NOTE allows click on typeahead to be accepted
            await PauseTypeahead();
        }

        isEditing = false;

        await RefreshMarkup();
    }

    private async Task OnClick(MouseEventArgs args)
    {
        await PauseTypeahead();
        await ClearSelectedText();
    }

    private async Task ClearSelectedText()
    {
        await Task.Yield();
        await GetTextAreaCaret();
    }

    private async Task OnMouseDown(MouseEventArgs args) => MouseSelectionStart();

    private async Task OnMouseMove(MouseEventArgs args)
    {
        await MouseSelecting();
        await AllowClickableMarkupWhenEditing();
    }

    private async Task OnMouseUp(MouseEventArgs args) => await MouseSelectionEnd();

    /// <summary>
    /// Called from js interop
    /// Note: performance improvement is do not bind keydown put get from interop call
    /// </summary>
    [JSInvokable]
    public async Task OnKeyDown(KeyboardEventArgs args)
    {
        isEditing = true;

        typeaheadPaused = false;
        TypeaheadHandleKeyDown(args);

        if (OnTypeaheadKeyDown.HasDelegate &&
            (typeaheadSuggestion.Any() || isCustomTypeaheadSuggestion))
        {
            await OnTypeaheadKeyDown.InvokeAsync(args);
            await GetTextAreaCaret();
            await UpdateTypeahead();
        }
    }

    /// <summary>
    /// Called from js interop to avoid clashing with consumer bound onkeyup event.
    /// </summary>
    [JSInvokable]
    public async Task OnKeyUp(KeyboardEventArgs args)
    {
        await GetTextAreaCaret();
        await UpdateTypeahead();
    }
}
