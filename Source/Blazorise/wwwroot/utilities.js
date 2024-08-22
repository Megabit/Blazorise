import "./vendors/jsencrypt.js?v=1.6.1.0";
import "./vendors/sha512.js?v=1.6.1.0";

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

    if (element && typeof element.focus === "function") {
        element.focus({
            preventScroll: !scrollToElement
        });
    }
}

// selects the given element
export function select(element, elementId, toFocus) {
    if (toFocus) {
        focus(element, elementId, true);
    }

    element = getRequiredElement(element, elementId);

    if (element && typeof element.select === "function") {
        element.select();
    }
}

// show a browser picker for the supplied input element
export function showPicker(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (element && 'showPicker' in HTMLInputElement.prototype) {
        element.showPicker();
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

export function scrollElementIntoView(elementId, smooth) {
    var element = document.getElementById(elementId);

    if (element) {
        var top;
        if (element.offsetTop < element.parentElement.scrollTop || element.clientHeight > element.parentElement.clientHeight) {
            top = element.offsetTop;
        } else if (element.offsetTop + element.offsetHeight > element.parentElement.scrollTop + element.parentElement.clientHeight) {
            top = element.offsetTop + element.offsetHeight - element.parentElement.clientHeight;
        }

        var scrollableParent = getScrollableParent(element);

        if (scrollableParent) {
            var behavior = smooth ? "smooth" : "instant";
            scrollableParent.scrollTo({ top: top, behavior: behavior });
        }
    }
}
function getScrollableParent(el) {
    while ((el = el.parentElement) && window.getComputedStyle(el).overflowY.indexOf('scroll') === -1);
    return el;
}

// sets the value to the element property
export function setProperty(element, property, value) {
    if (element && property) {
        element[property] = value;
    }
}

export function getElementInfo(element, elementId) {
    if (!element || (element && elementId && element.id !== elementId)) {
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

export function copyToClipboard(element, elementId) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    if (navigator.clipboard) {
        navigator.clipboard.writeText(element.innerText);
    }
}

function getExponentialParts(num) {
    return Array.isArray(num) ? num : String(num).split(/[eE]/);
}

function isExponential(num) {
    const eParts = getExponentialParts(num);
    return !Number.isNaN(Number(eParts[1]));
}

export function fromExponential(num) {
    if (!num)
        return num;

    const eParts = getExponentialParts(num);
    if (!isExponential(eParts)) {
        return eParts[0];
    }

    const sign = eParts[0][0] === '-' ? '-' : '';
    const digits = eParts[0].replace(/^-/, '');
    const digitsParts = digits.split('.');
    const wholeDigits = digitsParts[0];
    const fractionDigits = digitsParts[1] || '';
    let e = Number(eParts[1]);

    if (e === 0) {
        return `${sign + wholeDigits}.${fractionDigits}`;
    } else if (e < 0) {
        // move dot to the left
        const countWholeAfterTransform = wholeDigits.length + e;
        if (countWholeAfterTransform > 0) {
            // transform whole to fraction
            const wholeDigitsAfterTransform = wholeDigits.substr(0, countWholeAfterTransform);
            const wholeDigitsTransformedToFraction = wholeDigits.substr(countWholeAfterTransform);
            return `${sign + wholeDigitsAfterTransform}.${wholeDigitsTransformedToFraction}${fractionDigits}`;
        } else {
            // not enough whole digits: prepend with fractional zeros

            // first e goes to dotted zero
            let zeros = '0.';
            e = countWholeAfterTransform;
            while (e) {
                zeros += '0';
                e += 1;
            }
            return sign + zeros + wholeDigits + fractionDigits;
        }
    } else {
        // move dot to the right
        const countFractionAfterTransform = fractionDigits.length - e;
        if (countFractionAfterTransform > 0) {
            // transform fraction to whole
            // countTransformedFractionToWhole = e
            const fractionDigitsAfterTransform = fractionDigits.substr(e);
            const fractionDigitsTransformedToWhole = fractionDigits.substr(0, e);
            return `${sign + wholeDigits + fractionDigitsTransformedToWhole}.${fractionDigitsAfterTransform}`;
        } else {
            // not enough fractions: append whole zeros
            let zerosCount = -countFractionAfterTransform;
            let zeros = '';
            while (zerosCount) {
                zeros += '0';
                zerosCount -= 1;
            }
            return sign + wholeDigits + fractionDigits + zeros;
        }
    }
}

function getTag(value) {
    if (value == null) {
        return value === undefined ? '[object Undefined]' : '[object Null]'
    }
    return toString.call(value)
}

export function isString(value) {
    const type = typeof value
    return type === 'string' || (type === 'object' && value != null && !Array.isArray(value) && getTag(value) == '[object String]')
}

export function firstNonNull(value, fallbackValue) {
    if (value === null || value === undefined)
        return fallbackValue;

    return value;
}

export function verifyRsa(publicKey, content, signature) {
    try {
        const jsEncrypt = new JSEncrypt();
        jsEncrypt.setPublicKey(publicKey);

        const verified = jsEncrypt.verify(content, signature, sha512);

        if (verified) {
            return true;
        }
    } catch (error) {
        console.error(error);
    }

    return false;
}

export function log(message, args) {
    console.log(message, args);
}

export function createEvent(name) {
    const e = document.createEvent("Event");
    e.initEvent(name, true, true);
    return e;
}

export function coalesce(value, defaultValue) {
    return value === null || value === undefined ? defaultValue : value;
}