export function init(dotnetHelper, el, updateBatchCount, eventName) {

    el.previousText = el.value;

    if (eventName.startsWith('on')) {
        eventName = eventName.substring(2);
    }

    el.addEventListener(eventName, async e => {
        await sendChanges(dotnetHelper, el, updateBatchCount);
    }, false);
}

export async function sendChanges(dotnetHelper, el, updateBatchCount) {

    const getStart = () => {
        var i = 0;
        if (el.previousText === el.value) {
            return -1;
        }
        while (el.previousText[i] === el.value[i]) {
            i++;
        }
        return i;
    }

    const getEnd = () => {
        var i = 0;
        if (el.previousText === el.value) {
            return -1;
        }
        while (el.previousText[(el.previousText - 1) - i] === el.value[(el.value.length - 1) - i]
            && (el.value.length - 1) - i > 0) {
            i++;
        }
        return i;
    }

    let start = -1;
    let end = -1;
    let length = 0;
    let diff = null;
    let done = false;

    while (el.previousText !== el.value && !done) {

        done = true;

        diff = '';

        length = el.value.length - el.previousText.length;

        start = length === 1 ? el.selectionStart - 1 : getStart();
        end = length === 1 ? start + 1 : getEnd();

        if (length < 0) {
            el.previousText = el.previousText.slice(0, start) + el.previousText.slice(start - length);
        }
        else if (length > 0) {
            const batch = updateBatchCount === 0 ? length : Math.min(updateBatchCount, length);
            done = length <= updateBatchCount;
            diff = el.value.slice(start, start + batch);
            el.previousText = el.previousText.slice(0, start) + diff + el.previousText.slice(start);
        }

        await dotnetHelper.invokeMethodAsync('GetChange', start, length, diff, done);
    }
}

export async function setText(el, value) {
    el.previousText = value;
}