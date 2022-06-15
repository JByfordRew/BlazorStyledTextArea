using Microsoft.AspNetCore.Components.Web;

namespace BlazorStyledTextArea;

public partial class StyledTextArea 
{
    private string typeaheadSuggestion = "";
    private bool typeaheadPaused = false;
    private bool lastKeyWasTypeaheadReservedKey = false;
    private bool isCustomTypeaheadSuggestion = false;

    private async Task PauseTypeahead()
    {
        typeaheadPaused = true;
        await UpdateTypeahead();
        await GetTypeahead();
        await UpdateMarkupCurrentLine();
    }

    private async Task UpdateTypeahead()
    {
        var index = Math.Max(0, Math.Min(text.Length - 1, caretData.Index));
        char nextChar = index < text.Length - 1 ? text[index] : '\0';
        var isTypeaheadPaused = typeaheadPaused || (!char.IsWhiteSpace(nextChar) && !caretData.AtEndOfLine);
        var eventData = new CalculateTypeaheadEventData(index, caretData.AtEndOfLine, caretData.WordTextToCaret, nextChar, isTypeaheadPaused);
        await OnCalculateTypeahead.InvokeAsync(eventData);
        await GetTypeahead();
        await UpdateMarkupCurrentLine();
        await SetPreventDefaultOnKeyDown();
    }

    private async Task GetTypeahead()
    {
        var content = await jsInterop!.GetTypeaheadSuggestion(this.id.ToString());

        isCustomTypeaheadSuggestion = false;

        if (content == "[custom-typeahead]")
        {
            isCustomTypeaheadSuggestion = true;
            content = ".";
        }

        typeaheadSuggestion = content;
    }

    private void TypeaheadHandleKeyDown(KeyboardEventArgs args)
    {
        lastKeyWasTypeaheadReservedKey = false;

        if (this.IsTypeaheadReservedKey(args))
        {
            typeaheadPaused = false;
            lastKeyWasTypeaheadReservedKey = true;
        }
        else
        {
            switch (args.Key)
            {
                case "Shift":
                case "Control":
                case "Alt":
                    break;

                case "Backspace":
                    typeaheadPaused = false;
                    break;

                case "Escape":
                case "Delete":
                    typeaheadPaused = true;
                    break;

                default:
                    typeaheadPaused = args.Key.Length != 1;
                    break;
            }
        }
    }

    private async Task SetPreventDefaultOnKeyDown()
    {
        if (typeaheadPaused)
        {
            await jsInterop!.SetPreventDefaultOnKeyDown(TextArea, null!);
        }
        else 
        {
            await jsInterop!.SetPreventDefaultOnKeyDown(TextArea, TypeaheadReservedKeys!);
        }
    }

    public bool IsTypeaheadReservedKey(KeyboardEventArgs keyArgs) =>
        TypeaheadReservedKeys is not null &&
            TypeaheadReservedKeys.Any(x =>
                keyArgs.Key == x.Key &&
                keyArgs.AltKey == x.AltKey &&
                keyArgs.CtrlKey == x.CtrlKey &&
                keyArgs.MetaKey == x.MetaKey &&
                keyArgs.ShiftKey == x.ShiftKey);

    private string HiddenCaretCssClass => typeaheadSuggestion.Any() ? "hidden-caret" : "";
}
