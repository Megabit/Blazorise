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

export function setCaret(element, caret) {
    if (hasSelectionCapabilities(element)) {
        window.requestAnimationFrame(() => {
            element.selectionStart = caret;
            element.selectionEnd = caret;
        });
    }
}

export function getCaret(element) {
    return hasSelectionCapabilities(element)
        ? element.selectionStart :
        -1;
}

export function setTextValue(element, value) {
    element.value = value;
}

export function scrollAnchorIntoView(elementId) {
    var element = document.getElementById(elementId);

    if (element) {
        element.scrollIntoView();
        window.location.hash = elementId;
    }
}

// sets the value to the element property
export function setProperty(element, property, value) {
    if (element && property) {
        element[property] = value;
    }
}

export function getElementInfo(element, elementId) {
    if (!element) {
        element = document.getElementById(elementId);
    }

    if (element) {
        const position = element.getBoundingClientRect();

        return {
            boundingClientRect: {
                x: position.x,
                y: position.y,
                top: position.top,
                bottom: position.bottom,
                left: position.left,
                right: position.right,
                width: position.width,
                height: position.height
            },
            offsetTop: element.offsetTop,
            offsetLeft: element.offsetLeft,
            offsetWidth: element.offsetWidth,
            offsetHeight: element.offsetHeight,
            scrollTop: element.scrollTop,
            scrollLeft: element.scrollLeft,
            scrollWidth: element.scrollWidth,
            scrollHeight: element.scrollHeight,
            clientTop: element.clientTop,
            clientLeft: element.clientLeft,
            clientWidth: element.clientWidth,
            clientHeight: element.clientHeight
        };
    }

    return {};
}

function hasSelectionCapabilities(element) {
    const nodeName = element && element.nodeName && element.nodeName.toLowerCase();

    return (
        nodeName &&
        ((nodeName === 'input' &&
            (element.type === 'text' ||
                element.type === 'search' ||
                element.type === 'tel' ||
                element.type === 'url' ||
                element.type === 'password')) ||
            nodeName === 'textarea' ||
            element.contentEditable === 'true')
    );
}

export function getRequiredElement(element, elementId) {
    if (element)
        return element;

    return document.getElementById(elementId);
}


export function getUserAgent() {
    return navigator.userAgent;
}