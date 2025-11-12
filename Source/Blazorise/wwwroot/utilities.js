import "./vendors/jsencrypt.js?v=1.8.6.0";
import "./vendors/sha512.js?v=1.8.6.0";

// adds a classname to the specified element
export function addClass(element, classname) {
    if (element && element.classList) {
        element.classList.add(classname);
    }
}

// removes a classname from the specified element
export function removeClass(element, classname) {
    if (element && element.classList && element.classList.contains(classname)) {
        element.classList.remove(classname);
    }
}

// toggles a classname on the given element id
export function toggleClass(element, classname) {
    if (element && element.classList) {
        if (element.classList.contains(classname)) {
            element.classList.remove(classname);
        } else {
            element.classList.add(classname);
        }
    }
}

export function addAttribute(element, attribute, value) {
    if (element) {
        element.setAttribute(attribute, value);
    }
}

export function removeAttribute(element, attribute) {
    if (element) {
        element.removeAttribute(attribute);
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

// adds an attribute to the body element
export function addAttributeToBody(attribute, value) {
    addAttribute(document.body, attribute, value);
}

// removes an attribute from the body element
export function removeAttributeFromBody(attribute) {
    removeAttribute(document.body, attribute);
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

export function log(showBanner, message, args) {
    console.log(message, args);

    if (!showBanner) {
        return;
    }

    const HOST_ID = "blazorise-license-banner-host";
    const GLOBAL = "__blazoriseBannerState__";

    const st = (window[GLOBAL] ||= {
        dismissed: false,
        bodyObserver: null,
        attrObserver: null
    });

    if (st.dismissed) {
        return;
    }

    let cleanMessage = typeof message === "string" ? message.replace(/%c/g, "") : String(message);
    cleanMessage = cleanMessage
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#39;");

    let host = document.getElementById(HOST_ID);
    if (host && host.shadowRoot) {
        const msgEl = host.shadowRoot.querySelector(".msg");
        if (msgEl) msgEl.innerHTML = cleanMessage;
        return;
    }

    host = document.createElement("div");
    host.id = HOST_ID;
    const shadow = host.attachShadow({ mode: "open" });

    const style = document.createElement("style");
    style.textContent = `
:host { all: initial !important; }
.wrapper {
  position: fixed !important;
  left: 0 !important;
  right: 0 !important;
  bottom: 0 !important;
  z-index: 2147483647 !important;
  padding: 10px 14px !important;
  background: #6C63FF !important;
  color: #FFFFFF !important;
  font: 500 14px/1.4 -apple-system,BlinkMacSystemFont,Segoe UI,Roboto,Helvetica,Arial,sans-serif !important;
  box-shadow: 0 -4px 12px rgba(0,0,0,0.2) !important;
  display: flex !important;
  align-items: center !important;
  gap: .75rem !important;
  border-top-left-radius: 6px !important;
  border-top-right-radius: 6px !important;
}
.msg { flex: 1 1 auto !important; }
.btn {
  appearance: none !important;
  border: 1px solid rgba(255,255,255,0.7) !important;
  background: transparent !important;
  color: #FFFFFF !important;
  padding: .3rem .6rem !important;
  border-radius: .3rem !important;
  font-size: 12px !important;
  cursor: pointer !important;
  transition: background .2s ease-in-out, color .2s ease-in-out !important;
}
.btn:hover { background: rgba(255,255,255,0.2) !important; }
    `.trim();

    shadow.appendChild(style);

    const wrapperElement = document.createElement("div");
    wrapperElement.className = "wrapper";

    const messageElement = document.createElement("span");
    messageElement.className = "msg";
    messageElement.innerHTML = cleanMessage;

    const button = document.createElement("button");
    button.className = "btn";
    button.type = "button";
    button.textContent = "Dismiss";
    button.addEventListener("click", () => {
        st.dismissed = true;
        if (st.bodyObserver) try { st.bodyObserver.disconnect(); } catch { }
        if (st.attrObserver) try { st.attrObserver.disconnect(); } catch { }
        host.remove();
    });

    wrapperElement.appendChild(messageElement);
    wrapperElement.appendChild(button);
    shadow.appendChild(wrapperElement);
    document.body.appendChild(host);

    if (!st.bodyObserver) {
        st.bodyObserver = new MutationObserver(() => {
            if (st.dismissed) return;
            const exists = document.getElementById(HOST_ID);
            if (!exists) {
                try { document.body.appendChild(host); } catch { }
            }
        });
        st.bodyObserver.observe(document.body, { childList: true });
    }

    if (!st.attrObserver) {
        st.attrObserver = new MutationObserver(() => {
            if (st.dismissed) return;
            host.style.display = "block";
            host.style.visibility = "visible";
            host.style.opacity = "1";
        });
        st.attrObserver.observe(host, { attributes: true, attributeFilter: ["style", "class", "hidden"] });
    }
}

export function createEvent(name) {
    const e = document.createEvent("Event");
    e.initEvent(name, true, true);
    return e;
}

export function isNullOrUndefined(value) {
    return value === null || value === undefined;
}

export function coalesce(value, defaultValue) {
    return value === null || value === undefined ? defaultValue : value;
}

export function insertCSSIntoDocumentHead(url) {
    document.getElementsByTagName("head")[0].insertAdjacentHTML("beforeend", `<link rel=\"stylesheet\" href=\"${url}\" />`);
}

export function isSystemDarkMode() {
    return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
}