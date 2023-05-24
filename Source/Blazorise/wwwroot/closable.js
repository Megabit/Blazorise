let closableComponents = [];
let closableLightComponents = [];
let lastClickedDocumentElement = null;

function addClosableComponent(elementId, dotnetAdapter) {
    closableComponents.push({ elementId: elementId, dotnetAdapter: dotnetAdapter });
}

function addClosableLightComponent(elementId) {
    closableLightComponents.push({ elementId: elementId });
}

function isClosableComponent(elementId) {
    let index = 0;

    for (index = 0; index < closableComponents.length; ++index) {
        if (closableComponents[index].elementId === elementId)
            return true;
    }

    return false;
}

function isClosableLightComponent(elementId) {
    let index = 0;

    for (index = 0; index < closableLightComponents.length; ++index) {
        if (closableLightComponents[index].elementId === elementId)
            return true;
    }

    return false;
}

function findClosableComponentIndex(elementId) {
    let index = 0;

    for (index = 0; index < closableComponents.length; ++index) {
        if (closableComponents[index].elementId === elementId)
            return index;
    }

    return null;
}

function findClosableLightComponentIndex(elementId) {
    let index = 0;

    for (index = 0; index < closableLightComponents.length; ++index) {
        if (closableLightComponents[index].elementId === elementId)
            return index;
    }

    return null;
}

function tryClose(closable, targetElementId, isEscapeKey, isChildClicked) {
    let request = new Promise((resolve, reject) => {
        closable.dotnetAdapter.invokeMethodAsync('SafeToClose', targetElementId, isEscapeKey ? 'escape' : 'leave', isChildClicked)
            .then((result) => resolve({ elementId: closable.elementId, dotnetAdapter: closable.dotnetAdapter, status: result === true ? 'ok' : 'cancelled' }))
            .catch(() => resolve({ elementId: closable.elementId, status: 'error' }));
    });

    if (request) {
        request
            .then((response) => {
                if (response.status === 'ok') {
                    response.dotnetAdapter.invokeMethodAsync('Close', isEscapeKey ? 'escape' : 'leave')
                        // If the user navigates to another page then it will raise exception because the reference to the component cannot be found.
                        // In that case just remove the elementId from the list.
                        .catch(() => unregisterClosableComponent(response.elementId));
                }
            });
    }
}

function hasParentInTree(element, parentElementId) {
    if (!element.parentElement) return false;
    if (element.parentElement.id === parentElementId) return true;
    return hasParentInTree(element.parentElement, parentElementId);
}

function hasScroll(element) {
    return element.scrollHeight > element.clientHeight;
}

function scrollClick(event) {
    return hasScroll(event.target) && event.target.clientWidth < event.clientX;
}

export function registerClosableComponent(dotnetAdapter, element) {
    if (element) {
        if (isClosableComponent(element.id) !== true) {
            addClosableComponent(element.id, dotnetAdapter);
        }
    }
}

/// Adds a lighter closable component, that is tracked for close events.
export function registerClosableLightComponent(element) {
    if (element) {
        if (isClosableLightComponent(element.id) !== true) {
            addClosableLightComponent(element.id);
        }
    }
}

export function unregisterClosableLightComponent(element) {
    if (element) {
        const index = findClosableLightComponentIndex(element.id);
        if (index !== -1) {
            closableLightComponents.splice(index, 1);
        }
    }
}

export function unregisterClosableComponent(element) {
    if (element) {
        const index = findClosableComponentIndex(element.id);
        if (index !== -1) {
            closableComponents.splice(index, 1);
        }
    }
}

document.addEventListener('mousedown', function handler(evt) {
    lastClickedDocumentElement = evt.target;
});

document.addEventListener('mouseup', function handler(evt) {
    if (isClosableLightComponent(evt.target.id))
        return;

    if (evt.button === 0 && evt.target === lastClickedDocumentElement && closableComponents && closableComponents.length > 0) {
        const lastClosable = closableComponents[closableComponents.length - 1];
        const scrollClicked = scrollClick(evt);
        if (lastClosable && !scrollClicked) {
            tryClose(lastClosable, evt.target.id, false, hasParentInTree(evt.target, lastClosable.elementId));
        }
    }
});



document.addEventListener('keyup', function handler(evt) {
    if (isClosableLightComponent(evt.target.id))
        return;

    if (evt.keyCode === 27 && closableComponents && closableComponents.length > 0) {
        const lastClosable = closableComponents[closableComponents.length - 1];
        if (lastClosable) {
            tryClose(lastClosable, lastClosable.elementId, true, hasParentInTree(evt.target, lastClosable.elementId));
        }
    }
});