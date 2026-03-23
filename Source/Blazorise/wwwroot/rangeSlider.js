import { getRequiredElement } from "./utilities.js?v=2.0.3.0";

function getRangeSliderStep(element) {
    const step = Number.parseFloat(element?.step);

    if (Number.isNaN(step) || step <= 0) {
        return 1;
    }

    return step;
}

function clampRangeSliderValue(startElement, endElement, startHandle, clampToOtherHandle, allowEqualValues) {
    if (!clampToOtherHandle) {
        return;
    }

    const startValue = Number.parseFloat(startElement.value);
    const endValue = Number.parseFloat(endElement.value);
    const minimumDistance = allowEqualValues ? 0 : Math.max(getRangeSliderStep(startElement), getRangeSliderStep(endElement));

    if (Number.isNaN(startValue) || Number.isNaN(endValue)) {
        return;
    }

    if (startHandle && startValue > endValue - minimumDistance) {
        startElement.value = String(endValue - minimumDistance);
    } else if (!startHandle && endValue < startValue + minimumDistance) {
        endElement.value = String(startValue + minimumDistance);
    }
}

export function initialize(startElement, startElementId, endElement, endElementId, clampToOtherHandle, allowEqualValues) {
    startElement = getRequiredElement(startElement, startElementId);
    endElement = getRequiredElement(endElement, endElementId);

    if (!startElement || !endElement) {
        return;
    }

    const existingState = startElement.__blazoriseRangeSlider;

    if (existingState) {
        existingState.clampToOtherHandle = clampToOtherHandle;
        existingState.allowEqualValues = allowEqualValues;

        if (existingState.endElement === endElement) {
            clampRangeSliderValue(startElement, endElement, true, clampToOtherHandle, allowEqualValues);
            clampRangeSliderValue(startElement, endElement, false, clampToOtherHandle, allowEqualValues);
            return;
        }

        startElement.removeEventListener("input", existingState.onStartInput, true);
        existingState.endElement?.removeEventListener("input", existingState.onEndInput, true);
    }

    const state = {
        clampToOtherHandle: clampToOtherHandle,
        allowEqualValues: allowEqualValues,
        endElement: endElement,
        onStartInput: null,
        onEndInput: null
    };

    state.onStartInput = () => clampRangeSliderValue(startElement, endElement, true, state.clampToOtherHandle, state.allowEqualValues);
    state.onEndInput = () => clampRangeSliderValue(startElement, endElement, false, state.clampToOtherHandle, state.allowEqualValues);

    startElement.addEventListener("input", state.onStartInput, true);
    endElement.addEventListener("input", state.onEndInput, true);
    startElement.__blazoriseRangeSlider = state;

    clampRangeSliderValue(startElement, endElement, true, clampToOtherHandle, allowEqualValues);
    clampRangeSliderValue(startElement, endElement, false, clampToOtherHandle, allowEqualValues);
}

export function destroy(startElement, startElementId) {
    startElement = getRequiredElement(startElement, startElementId);

    const state = startElement?.__blazoriseRangeSlider;

    if (!state) {
        return;
    }

    startElement.removeEventListener("input", state.onStartInput, true);
    state.endElement?.removeEventListener("input", state.onEndInput, true);
    delete startElement.__blazoriseRangeSlider;
}