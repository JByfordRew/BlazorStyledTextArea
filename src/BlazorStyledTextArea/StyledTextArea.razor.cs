using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorStyledTextArea;

public partial class StyledTextArea : ComponentBase
{
    private string textareaClass => $"just-content overlayed hidden-text {HiddenCaretCssClass}";

    private readonly Guid id = Guid.NewGuid();

    private JsInterop? jsInterop;

    [Inject]
    internal IJSRuntime? JsRuntime { get; set; }

    private IDictionary<string, object> additionalAttributes = new Dictionary<string,object>();

    private string eventCallbackName = "";
    private EventCallback<string> eventCallback;

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? AdditionalAttributes {
        get
        {
            return additionalAttributes;
        }
        set
        {
            if (value is not null && value.Any(_ => _.Value is EventCallback<string>))
            {
                var eventCallbackAttribute = value.Last(_ => _.Value is EventCallback<string>);
                eventCallbackName = eventCallbackAttribute.Key;
                eventCallback = (EventCallback<string>)eventCallbackAttribute.Value;
                value = value.Where(_ => _.Value is not EventCallback<string>)
                    .ToDictionary(_ => _.Key, _ => _.Value);
            }

            additionalAttributes = value;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await UpdateMarkupFromText(text);
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            jsInterop = new JsInterop(JsRuntime!);
            var objRef = DotNetObjectReference.Create(this);
            await jsInterop.Init(objRef, TextArea, id.ToString(), eventCallbackName);
        }
        
        await DeferredSetCaret();
        await UpdateCursorPosition();
        await UpdateCaretData();

        await base.OnAfterRenderAsync(firstRender);
    }
}
