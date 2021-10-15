// adds a classname to the specified element
export function addClass(element, classname) {
    element.classList.add(classname);
}

// removes a classname from the specified element
export function removeClass(element, classname) {
    if (element.classList.contains(classname)) {
        element.classList.remove(classname);
    }
}

// toggles a classname on the given element id
export function toggleClass(element, classname) {
    if (element) {
        if (element.classList.contains(classname)) {
            element.classList.remove(classname);
        } else {
            element.classList.add(classname);
        }
    }
}

// adds a classname to the body element
export function addClassToBody(classname) {
    addClass(document.body, classname);
}

// removes a classname from the body element
export function removeClassFromBody(classname) {
    removeClass(document.body, classname);
}

// sets the input focuses to the given element
export function focus(element, elementId, scrollToElement) {
    element = getRequiredElement(element, elementId);

    if (element) {
        element.focus({
            preventScroll: !scrollToElement
        });
    }
}

// selects the given element
export function select(element, elementId, focus) {
    if (focus) {
        focus(element, elementId, true);
    }

    element = getRequiredElement(element, elementId);

    if (element) {
        element.select();
    }
}

function getRequiredElement(element, elementId) {
    if (element)
        return element;

    return document.getElementById(elementId);
}