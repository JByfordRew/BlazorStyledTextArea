using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorStyledTextArea;

public partial class StyledTextArea 
{
    #region Text

    private string text = "";
    
    private string Text
    {
        get => text;
        set => InvokeAsync(async () => await UpdateText(value));
    }

    private string _value = null!;

    /// <summary>
    /// Text string to bind to textarea.
    /// </summary>
    [Parameter]
    public string Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                Text = _value;
            }
        }
    }

    /// <summary>
    /// Bound Text setter callback.
    /// </summary>
    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// Js interop invoked method to callback to the consumer with the current text.
    /// </summary>
    /// <returns>Current state of edited text</returns>
    [JSInvokable]
    public async Task Callback()
    {
        await eventCallback.InvokeAsync(text);
    }

    /// <summary>
    /// Recommended to correct line endings chars to \n from \r\n 
    /// </summary>
    /// <param name="text">text to correct line endings</param>
    /// <returns>string with line endings as \n</returns>
    public static string PrepareText(string text) => text.Replace("\r\n", "\n");

    #endregion

    /// <summary>
    /// Apply styles to typed text automatically via StyleRules.
    /// </summary>
    [Parameter]
    public List<StyleRule> StyleRules { get; set; } = new List<StyleRule>();

    private CaretData caretData = new(0, 0, 0, 0, 0, 0, false, "");

    /// <summary>
    /// Get updates to caret data including text index and length, row and column, x and y pixel coords, is caret at end of line and word typed to caret.
    /// </summary>
    [Parameter]
    public EventCallback<CaretData> OnCaretChanged { get; set; }

    /// <summary>
    /// All changed lines with start index and length.  Also includes expired StyleRules to remove and recalculate.
    /// </summary>
    [Parameter]
    public EventCallback<ChangedLines> OnLinesChanged { get; set; }

    /// <summary>
    /// Callback when styled text with and Id clicked (when not currently editing textarea, not having focus)
    /// </summary>
    [Parameter]
    public EventCallback<Element> OnElementClicked { get; set; }

    /// <summary>
    /// Styles with Id can be clickable after mouse moved in milliseconds before return to editing.
    /// </summary>
    [Parameter]
    public int TimeElementIsClickableInMilliseconds { get; set; } = 1200;

    /// <summary>
    /// Choose not to use the improved Textarea that provides better performance in Web Assembly (not AOT).
    /// Best to use standard textarea if using Blazor Server or Web Assembly with AOT.
    /// </summary>
    [Parameter]
    public bool UseStandardTextarea { get; set; } = false;

    #region Typeahead 

    /// <summary>
    /// Provide inline typeahead behaviour
    /// </summary>
    [Parameter]
    public RenderFragment? InlineTypeahead { get; set; }

    /// <summary>
    /// Provide custom typeahead behaviour usually and overlayed element
    /// </summary>
    [Parameter]
    public RenderFragment? CustomTypeahead { get; set; }

    /// <summary>
    /// Info on each key down event.  useful to choose to accept typeahead suggestion.
    /// </summary>
    [Parameter]
    public EventCallback<KeyboardEventArgs> OnTypeaheadKeyDown { get; set; }

    /// <summary>
    /// Info useful to help calculate typeahead suggestion based on caret position and context
    /// </summary>
    [Parameter]
    public EventCallback<CalculateTypeaheadEventData> OnCalculateTypeahead { get; set; }

    /// <summary>
    /// Specify list of reserve keys when typeahead is being displayed, which are used by the typeahead.
    /// For instance the key used to accept the typeahead suggestion or changed focus to the list of suggestions
    /// </summary>
    [Parameter]
    public IEnumerable<KeyboardEventArgs>? TypeaheadReservedKeys { get; set; }

    /// <summary>
    /// Used to stop key presses from being processed by other elements. 
    /// e.g. when typeahead suggestion 'enter' is pressed to select option then 'enter' is not passed to textarea.
    /// </summary>
    /// <param name="elementReference">Any element reference</param>
    /// <param name="keyArgs">List of keys reserved by this element</param>
    /// <returns></returns>
    public async Task PreventDefaultOnKeyDown(ElementReference elementReference, IEnumerable<KeyboardEventArgs> keyArgs)
    {
        await jsInterop!.SetPreventDefaultOnKeyDown(elementReference, keyArgs);
        await jsInterop!.AddPreventDefaultOnKeyDown(elementReference);
    }

    #endregion

    /// <summary>
    /// Force a text markup to be updated.
    /// </summary>
    /// <returns></returns>
    public async Task Refresh() => await RefreshMarkup();
}
