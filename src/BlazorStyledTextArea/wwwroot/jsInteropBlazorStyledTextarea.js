export function init(dotnetHelper, component, id, eventCallbackName) {

    window.blazorStyledTextArea = window.blazorStyledTextArea || {};

    window.blazorStyledTextArea.helpers = window.blazorStyledTextArea.helpers || {};
    window.blazorStyledTextArea.helpers[id] = dotnetHelper;

    window.blazorStyledTextArea.preventDefaultKeyArgs = window.blazorStyledTextArea.preventDefaultKeyArgs || {};
    window.blazorStyledTextArea.preventDefaultKeyArgs[component] = [{}];

    window.blazorStyledTextArea.clicked = function (el) {
        let elementId = el.id;
        let text = el.innerText;

        while (!el.classList.contains('textarea-container')) {
            el = el.parentElement;
        }
        let componentId = el.id;

        let helper = window.blazorStyledTextArea.helpers[componentId];

        return helper.invokeMethodAsync('MarkupClicked', elementId, text);
    }

    window.blazorStyledTextArea.mousemoved = function (el) {
        if (new Date(Date.now()).getMilliseconds() < 750) {
            let elementId = el.id;

            while (!el.classList.contains('textarea-container')) {
                el = el.parentElement;
            }
            let componentId = el.id;

            let helper = window.blazorStyledTextArea.helpers[componentId];

            return helper.invokeMethodAsync('MarkupMouseMoved', elementId);
        }
    }

    component.addEventListener('keydown', async e => {
        const args = {
            key: e.key,
            code: e.code,
            location: e.location,
            repeat: e.repeat,
            ctrlKey: e.ctrlKey,
            shiftKey: e.shiftKey,
            altKey: e.altKey,
            metaKey: e.metaKey,
            type: e.type
        };
        await dotnetHelper.invokeMethodAsync('OnKeyDown', args);
    }, false);

    if (eventCallbackName !== 'oninput') {
        if (eventCallbackName.startsWith('on')) {
            eventCallbackName = eventCallbackName.substring(2);
        }

        component.addEventListener(eventCallbackName, async e => {
            await dotnetHelper.invokeMethodAsync('Callback');
        }, false);
    }

    preventDefaultOnKeyDown(component, false);
}

export function setPreventDefaultOnKeyDown(element, keyArgs) {
    window.blazorStyledTextArea.preventDefaultKeyArgs[element] = keyArgs;
}

export function getTypeaheadSuggestion(id) {
    let content = document.getElementById('id-' + id + '-bsta-typeahead')?.innerText ?? '';
    if (content.length === 0) {
        let custom = document.querySelector('#id-' + id + '-bsta-typeahead + *')?.innerHTML ?? '';
        content = custom.length > 0 ? '[custom-typeahead]' : '';
    }
    return content;
}

export function focusTextArea(el) {
    el.focus();
}

export function blurTextArea(el) {
    el.blur();
}

export function setTextAreaCaret(el, caret) {
    if (el && caret >= 0) {
        el.selectionStart = caret;
        el.selectionEnd = caret;
    }
}

export function insertTextAreaValue(el, caret, wordLength, value) {

    el.value = el.value.slice(0, caret - wordLength) + value + el.value.slice(caret);
}

export function getTextAreaPosColRow(el) {
    if (el) {
        let getRowCol = function (p) {
            let rows = el.value.substr(0, p).split("\n");
            rows.pop();
            let row = rows.length;
            let rowsChars = rows.join("\n").length;
            let firstRowOffset = row === 0 ? 0 : 1;
            let col = p - rowsChars - firstRowOffset;
            return { 'col': col, 'row': row };
        }

        let start = getRowCol(el.selectionStart);
        let end = getRowCol(el.selectionEnd);

        return [el.selectionStart, start.col, start.row, end.col, end.row, el.length ?? 0];
    }
}

export function getElementPosition(componentId, id) {
    let el = document.getElementById(componentId).querySelector('#'+id);
    if (el) {
        let bodyRect = document.body.getBoundingClientRect();
        let elemRect = el.getBoundingClientRect();
        let top = Math.round(elemRect.top - bodyRect.top);
        let left = Math.round(elemRect.left - bodyRect.left);
        return [left, top];
    }
    return [0, 0];
}

export function preventDefaultOnKeyDown(element, remove = false) {
    let preventDefaultFunction = function (e) {
        let keyArgs = window.blazorStyledTextArea.preventDefaultKeyArgs[element];
        for (var i = 0; i < keyArgs?.length; i++) {
            let keyArg = keyArgs[i] || {};
            let keyMatch = e.key === keyArg.key &&
                e.altKey === keyArg.altKey &&
                e.ctrlKey === keyArg.ctrlKey &&
                e.metaKey === keyArg.metaKey &&
                e.shiftKey === keyArg.shiftKey;
            if (keyMatch) {
                e.preventDefault();
                return false;
            }
        }
    }
    if (remove) {
        element?.removeEventListener('keydown', preventDefaultFunction, false);
    }
    else {
        element?.addEventListener('keydown', preventDefaultFunction, false);
    }
}

export function timeStart(name) { console.time(name) }
export function timeEnd(name) { console.timeEnd(name); }

