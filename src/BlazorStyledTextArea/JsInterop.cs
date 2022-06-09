using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Runtime.CompilerServices;

namespace BlazorStyledTextArea;

public class JsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    private const bool doProfile = false;
    private const bool profileStackTrace = false;

    public JsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
           "import", "./_content/BlazorStyledTextArea/jsInteropBlazorStyledTextarea.js").AsTask());
    }

    internal async Task Profile(Func<Task> task, string name = "", [CallerMemberName] string caller = "")
    {
        if (doProfile)
        {
            var trace = profileStackTrace ? new System.Diagnostics.StackTrace().ToString() : "";
            name = caller + "-" + name + " " + trace;
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("timeStart", name);
            await task();
            await module.InvokeVoidAsync("timeEnd", name);
        }
        else
        {
            await task();
        }
    }

    internal async Task<T> Profile<T>(Func<Task<T>> task, string name = "", [CallerMemberName] string caller = "")
    {
        if (doProfile)
        {
            var trace = profileStackTrace ? new System.Diagnostics.StackTrace().ToString() : "";
            name = caller + "-" + name + " " + trace;
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("timeStart", name);
            T result = await task();
            await module.InvokeVoidAsync("timeEnd", name);
            return result;
        }
        else
        {
            return await task();
        }
    }

    internal async Task Init<T>(T objRef, ElementReference elementReference, string id, string eventCallbackName)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("init", objRef, elementReference, id, eventCallbackName);
    }

    internal async Task Debug(string t)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("debug", t);
    }

    internal async Task FocusTextArea(ElementReference elementReference)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("focusTextArea", elementReference);
    }

    internal async Task BlurTextArea(ElementReference elementReference)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("blurTextArea", elementReference);
    }

    internal async ValueTask<List<int>> GetTextAreaCaret(ElementReference elementReference)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<List<int>>("getTextAreaPosColRow", elementReference);
    }

    internal async Task SetTextAreaCaret(ElementReference elementReference, int caret)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("setTextAreaCaret", elementReference, caret);
    }

    internal async Task InsertTextAreaValue(ElementReference elementReference, int caretPosition, int wordLength, string value)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("insertTextAreaValue", elementReference, caretPosition, wordLength, value);
    }

    internal async ValueTask<List<decimal>> GetElementPosition(string componentId, string elemendId)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<List<decimal>>("getElementPosition", componentId, elemendId);
    }
    
    internal async ValueTask<string> GetTypeaheadSuggestion(string id)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<string>("getTypeaheadSuggestion", id);
    }

    internal async Task AddPreventDefaultOnKeyDown(ElementReference elementReference)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("preventDefaultOnKeyDown", elementReference, false);
    }

    internal async Task SetPreventDefaultOnKeyDown(ElementReference elementReference, IEnumerable<KeyboardEventArgs> keyArgs)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("setPreventDefaultOnKeyDown", elementReference, keyArgs);
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }
}
