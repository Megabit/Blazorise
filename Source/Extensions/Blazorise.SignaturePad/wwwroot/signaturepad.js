import { getRequiredElement } from "../Blazorise/utilities.js?v=1.2.2.0";
import "./vendors/sigpad.js?v=1.2.2.0";

const _instances = [];

export function initialize(dotNetAdapter, element, elementId, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const sigpad = new SignaturePad(element, {
        minWidth: 5,
        maxWidth: 10,
        penColor: "rgb(66, 133, 244)"
    });

    //sigpad.fromData(options.value, { clear: false });

    sigpad.addEventListener("endStroke", (e) => {
        dotNetAdapter.invokeMethodAsync("NotifyValue", sigpad.toDataURL("image/png"))
            .catch((reason) => {
                console.error(reason);
            });
       // console.log(sigpad.toDataURL("image/png"));
    });

    const instance = {
        options: options,
        sigpad: sigpad,
    };

    registerToEvents(dotNetAdapter, instance.sigpad);

    _instances[elementId] = instance;
}
export function destroy(element, elementId) {
    const instances = _instances || {};
    const instance = instances[elementId];
    if (instance) {
        if (instance.sigpad) {
            instance.sigpad.destroy();
        }

        delete instances[elementId];
    }
}
export function updateOptions(element, elementId, options) {
    const instance = _instances[elementId];
    if (instance && instance.sigpad && options) {
        if (options.source.changed) {
            updateSource(element, elementId, options.source.value);
        }
    }
}

export function setData(element, elementId, data) {
    const instance = _instances[elementId];
    if (instance && instance.sigpad) {
        instance.sigpad.fromData(data);
    }
}


function registerToEvents(dotNetAdapter, sigpad) {
    sigpad.onEnd = () => {
        const data = sigpad.toDataURL();
        dotNetAdapter.invokeMethodAsync("OnEnd", data);
    };
    sigpad.onBegin = () => {
        dotNetAdapter.invokeMethodAsync("OnBegin");
    };
    sigpad.onClear = () => {
        dotNetAdapter.invokeMethodAsync("OnClear");
    };
}

function updateSource(element, elementId, source) {
    const instance = _instances[elementId];
    if (instance && instance.sigpad) {
        instance.sigpad.clear();
    }
}

