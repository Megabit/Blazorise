///https://developer.mozilla.org/en-US/docs/Web/API/MutationObserver

const _instances = [];

///Creates a new observer on a given element, please follow the link below for usage
///https://developer.mozilla.org/en-US/docs/Web/API/MutationObserver/observe
export function createObserver(elementId, callback, configuration) {
    const observer = new MutationObserver(callback);
    observer.observe(document.getElementById(elementId), configuration);
    _instances[elementId] = observer;
    return observer;
}

///Creates a new observer that's setup to listen for attribute changes on a given element.
export function createAttributesObserver(targetNode, callback) {
    return createObserver(targetNode, callback, { attributes: true, attributeOldValue: true });
}

///Observer helper function, sets an observer callback based on a class name
export function observeClassChanged(mutationsList, className, onChangedCallBack, triggerOnOldValue) {
    if (mutationsList && className) {
        for (const mutation of mutationsList) {
            if (mutation.type === 'attributes' && mutation.attributeName === 'class' && (mutation.target.classList.contains(className)) || (triggerOnOldValue && mutation.oldValue && mutation.oldValue.includes(className))) {
                if (typeof (onChangedCallBack) === "function")
                    onChangedCallBack();
            }
        }
    }
}

///Observer helper function, sets an observer callback based on an attribute name
export function observeAttributeChanged(mutationsList, attributeName, onChangedCallBack) {
    if (mutationsList && attributeName) {
        for (const mutation of mutationsList) {
            if (mutation.type === 'attributes' && mutation.attributeName === 'attribute' && mutation.target.classList.contains(attributeName)) {
                if (typeof (onChangedCallBack) === "function")
                    onChangedCallBack();
            }
        }
    }
}

///Stops and clean ups the observer
export function destroyObserver(elementId) {
    const instances = _instances || {};

    const instance = instances[elementId];

    if (instance) {
        instance.disconnect();

        delete instances[elementId];
    }
}