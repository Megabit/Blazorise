import "./vendors/jsencrypt.js?v=2.2.2.0";
import "./vendors/sha512.js?v=2.2.2.0";

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

export function submitClosestForm(element) {
    const form = element && typeof element.closest === "function"
        ? element.closest("form")
        : null;

    if (!form) {
        return;
    }

    if (typeof form.requestSubmit === "function") {
        form.requestSubmit();
    } else {
        const submitEvent = new Event("submit", { bubbles: true, cancelable: true });

        if (form.dispatchEvent(submitEvent)) {
            form.submit();
        }
    }
}

export function dispatchKeyboardEvent(element, eventName, key, code, keyCode) {
    if (!element) {
        return true;
    }

    const event = new KeyboardEvent(eventName, {
        key: key,
        code: code,
        bubbles: true,
        cancelable: true
    });

    Object.defineProperty(event, "keyCode", { get: () => keyCode });
    Object.defineProperty(event, "which", { get: () => keyCode });

    return element.dispatchEvent(event) && !event.defaultPrevented;
}

export function setCaret(element, caret) {
    if (hasSelectionCapabilities(element)) {
        window.requestAnimationFrame(() => {
            element.selectionStart = caret;
            element.selectionEnd = caret;
        });
    } else if (isNumberInput(element)) {
        numericInputCarets.set(element, caret);
    }
}

export function getCaret(element) {
    return getSelection(element).start;
}

export function getSelection(element) {
    if (isNumberInput(element)) {
        const caret = numericInputCarets.has(element)
            ? numericInputCarets.get(element)
            : `${element.value || ''}`.length;

        return { start: caret, end: caret };
    }

    if (hasSelectionCapabilities(element) &&
        typeof element.selectionStart === 'number' &&
        typeof element.selectionEnd === 'number') {
        return {
            start: element.selectionStart,
            end: element.selectionEnd
        };
    }

    return { start: -1, end: -1 };
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

export function scrollElementIntoViewForOnScreenKeyboard(elementId, keyboardElementId, margin) {
    const element = document.getElementById(elementId);
    const keyboardElement = document.getElementById(keyboardElementId);

    if (!element || !keyboardElement) {
        return;
    }

    window.requestAnimationFrame(() => {
        const elementRect = element.getBoundingClientRect();
        const keyboardRect = keyboardElement.getBoundingClientRect();

        if (!elementRect.width || !elementRect.height || !keyboardRect.width || !keyboardRect.height) {
            return;
        }

        const safeMargin = Number.isFinite(margin) ? margin : 12;
        let scrollDelta = 0;

        if (keyboardRect.top > window.innerHeight / 2) {
            const coveredByBottomKeyboard = elementRect.bottom + safeMargin - keyboardRect.top;

            if (coveredByBottomKeyboard > 0) {
                scrollDelta = coveredByBottomKeyboard;
            }
        } else {
            const coveredByTopKeyboard = keyboardRect.bottom + safeMargin - elementRect.top;

            if (coveredByTopKeyboard > 0) {
                scrollDelta = -coveredByTopKeyboard;
            }
        }

        if (!scrollDelta) {
            return;
        }

        const scrollableParent = getScrollableParentForOnScreenKeyboard(element);
        const adjustment = Math.ceil(keyboardRect.height + safeMargin);
        const behavior = prefersReducedMotion() ? "auto" : "smooth";

        applyOnScreenKeyboardScrollAdjustment(scrollableParent, adjustment);

        if (isElementScrollTarget(scrollableParent)) {
            scrollableParent.scrollBy({ top: scrollDelta, behavior: behavior });
        } else {
            getDocumentScrollTarget().scrollBy({ top: scrollDelta, behavior: behavior });
        }
    });
}

export function clearOnScreenKeyboardScrollAdjustment() {
    if (!onScreenKeyboardScrollAdjustmentTarget) {
        return;
    }

    onScreenKeyboardScrollAdjustmentTarget.style.paddingBottom = onScreenKeyboardOriginalPaddingBottom;
    onScreenKeyboardScrollAdjustmentTarget = null;
    onScreenKeyboardOriginalPaddingBottom = null;
    onScreenKeyboardOriginalComputedPaddingBottom = 0;
}

function getScrollableParent(el) {
    while ((el = el.parentElement) && window.getComputedStyle(el).overflowY.indexOf('scroll') === -1);
    return el;
}

function getScrollableParentForOnScreenKeyboard(el) {
    while ((el = el.parentElement) && !isScrollableElement(el));
    return el;
}

function isScrollableElement(element) {
    const style = window.getComputedStyle(element);
    const overflowY = `${style.overflowY} ${style.overflow}`;

    return /(auto|scroll|overlay)/.test(overflowY) && element.scrollHeight > element.clientHeight;
}

function prefersReducedMotion() {
    return window.matchMedia && window.matchMedia("(prefers-reduced-motion: reduce)").matches;
}

function getDocumentScrollTarget() {
    return document.scrollingElement || document.documentElement || document.body;
}

function isElementScrollTarget(element) {
    return element && element !== document.body && element !== document.documentElement && element !== getDocumentScrollTarget();
}

let onScreenKeyboardScrollAdjustmentTarget = null;
let onScreenKeyboardOriginalPaddingBottom = null;
let onScreenKeyboardOriginalComputedPaddingBottom = 0;

function applyOnScreenKeyboardScrollAdjustment(scrollableParent, adjustment) {
    const target = isElementScrollTarget(scrollableParent)
        ? scrollableParent
        : getDocumentScrollTarget();

    if (!target) {
        return;
    }

    if (onScreenKeyboardScrollAdjustmentTarget && onScreenKeyboardScrollAdjustmentTarget !== target) {
        clearOnScreenKeyboardScrollAdjustment();
    }

    if (!onScreenKeyboardScrollAdjustmentTarget) {
        onScreenKeyboardScrollAdjustmentTarget = target;
        onScreenKeyboardOriginalPaddingBottom = target.style.paddingBottom || "";
        onScreenKeyboardOriginalComputedPaddingBottom = parseFloat(window.getComputedStyle(target).paddingBottom) || 0;
    }

    const currentPaddingBottom = parseFloat(window.getComputedStyle(target).paddingBottom) || 0;
    const paddingBottom = Math.max(currentPaddingBottom, onScreenKeyboardOriginalComputedPaddingBottom + adjustment);

    target.style.paddingBottom = `${paddingBottom}px`;
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

const numericInputCarets = new WeakMap();
const numericCaretCanvas = document.createElement('canvas');
const numericCaretContext = numericCaretCanvas.getContext('2d');

document.addEventListener('pointerup', event => {
    const element = event.target;

    if (isNumberInput(element)) {
        numericInputCarets.set(element, estimateNumberInputCaret(element, event.clientX));
    }
}, true);

function isNumberInput(element) {
    return element && element.nodeName && element.nodeName.toLowerCase() === 'input' && element.type === 'number';
}

function estimateNumberInputCaret(element, clientX) {
    const value = `${element.value || ''}`;

    if (value.length === 0)
        return 0;

    const style = window.getComputedStyle(element);
    const rect = element.getBoundingClientRect();
    const paddingLeft = parseFloat(style.paddingLeft) || 0;
    const paddingRight = parseFloat(style.paddingRight) || 0;
    const borderLeft = parseFloat(style.borderLeftWidth) || 0;
    const borderRight = parseFloat(style.borderRightWidth) || 0;
    const contentLeft = rect.left + borderLeft + paddingLeft;
    const contentRight = rect.right - borderRight - paddingRight;
    const contentWidth = Math.max(0, contentRight - contentLeft);

    numericCaretContext.font = `${style.fontStyle} ${style.fontVariant} ${style.fontWeight} ${style.fontSize} ${style.fontFamily}`;

    const textWidth = numericCaretContext.measureText(value).width;
    const align = style.textAlign;
    let textLeft = contentLeft;

    if (align === 'right' || align === 'end') {
        textLeft = contentRight - textWidth;
    } else if (align === 'center') {
        textLeft = contentLeft + (contentWidth - textWidth) / 2;
    }

    const x = Math.max(0, clientX - textLeft);
    let bestIndex = 0;
    let bestDistance = Math.abs(x);

    for (let index = 1; index <= value.length; index++) {
        const width = numericCaretContext.measureText(value.substring(0, index)).width;
        const distance = Math.abs(x - width);

        if (distance < bestDistance) {
            bestDistance = distance;
            bestIndex = index;
        }
    }

    return bestIndex;
}

export function getRequiredElement(element, elementId) {
    if (element)
        return element;

    return document.getElementById(elementId);
}

const disconnectCleanupRegistry = new Map();
let disconnectCleanupObserver = null;
let nextDisconnectCleanupId = 0;

function ensureDisconnectCleanupObserver() {
    if (disconnectCleanupObserver || !document.body) {
        return;
    }

    disconnectCleanupObserver = new MutationObserver(() => {
        for (const [cleanupId, cleanupEntry] of disconnectCleanupRegistry) {
            if (!cleanupEntry.element || !cleanupEntry.element.isConnected) {
                disconnectCleanupRegistry.delete(cleanupId);

                try {
                    cleanupEntry.cleanup();
                }
                catch (error) {
                    console.error(error);
                }
            }
        }
    });

    disconnectCleanupObserver.observe(document.body, { childList: true, subtree: true });
}

export function registerDisconnectCleanup(element, cleanup) {
    if (!element || typeof cleanup !== "function") {
        return null;
    }

    ensureDisconnectCleanupObserver();

    const cleanupId = ++nextDisconnectCleanupId;
    disconnectCleanupRegistry.set(cleanupId, { element, cleanup });

    return cleanupId;
}

export function unregisterDisconnectCleanup(cleanupId) {
    if (cleanupId === null || cleanupId === undefined) {
        return;
    }

    disconnectCleanupRegistry.delete(cleanupId);
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

export function log(showBanner, showCompactBanner, message, args) {
    if (typeof showCompactBanner !== "boolean") {
        args = message;
        message = showCompactBanner;
        showCompactBanner = false;
    }

    console.log(message, args);

    if (!showBanner && !showCompactBanner) {
        return;
    }

    const HOST_ID = "blazorise-license-banner-host";
    const GLOBAL = "__blazoriseBannerState__";

    const st = (window[GLOBAL] ||= {
        dismissed: false,
        expanded: false,
        compactAvailable: false,
        bodyObserver: null,
        attrObserver: null
    });

    st.compactAvailable = showCompactBanner;

    if (st.compactAvailable) {
        st.dismissed = false;
    }

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
        const wrapperEl = host.shadowRoot.querySelector(".wrapper");
        if (wrapperEl) wrapperEl.classList.toggle("compact", showCompactBanner && !st.expanded);
        const compactButtonEl = host.shadowRoot.querySelector(".compact-trigger");
        if (compactButtonEl) compactButtonEl.setAttribute("aria-expanded", st.expanded ? "true" : "false");
        const dismissButtonEl = host.shadowRoot.querySelector(".dismiss");
        if (dismissButtonEl) dismissButtonEl.textContent = st.compactAvailable ? "Close" : "Dismiss";
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
.wrapper.compact {
  left: auto !important;
  right: 16px !important;
  bottom: 16px !important;
  padding: 0 !important;
  background: rgba(108,99,255,0.72) !important;
  border-radius: 999px !important;
  box-shadow: 0 4px 14px rgba(0,0,0,0.24) !important;
  transition: background .2s ease-in-out !important;
}
.wrapper.compact:hover { background: #6C63FF !important; }
.msg { flex: 1 1 auto !important; }
.compact .msg,
.compact .dismiss { display: none !important; }
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
.compact-trigger {
  display: none !important;
  appearance: none !important;
  border: 0 !important;
  background: transparent !important;
  color: #FFFFFF !important;
  padding: .55rem .85rem !important;
  border-radius: 999px !important;
  font: 600 12px/1.2 -apple-system,BlinkMacSystemFont,Segoe UI,Roboto,Helvetica,Arial,sans-serif !important;
  cursor: pointer !important;
  white-space: nowrap !important;
  transition: background .2s ease-in-out !important;
}
.compact .compact-trigger { display: block !important; }
.compact-trigger:hover { background: rgba(255,255,255,0.2) !important; }
.btn:focus-visible,
.compact-trigger:focus-visible { outline: 2px solid #FFFFFF !important; outline-offset: 2px !important; }
    `.trim();

    shadow.appendChild(style);

    const wrapperElement = document.createElement("div");
    wrapperElement.className = "wrapper";
    wrapperElement.classList.toggle("compact", showCompactBanner && !st.expanded);
    wrapperElement.setAttribute("role", "region");
    wrapperElement.setAttribute("aria-label", "Blazorise license information");

    const messageElement = document.createElement("span");
    messageElement.className = "msg";
    messageElement.id = "blazorise-license-banner-message";
    messageElement.innerHTML = cleanMessage;

    const compactButton = document.createElement("button");
    compactButton.className = "compact-trigger";
    compactButton.type = "button";
    compactButton.textContent = "Community License";
    compactButton.setAttribute("aria-controls", messageElement.id);
    compactButton.setAttribute("aria-expanded", "false");

    const button = document.createElement("button");
    button.className = "btn dismiss";
    button.type = "button";
    button.textContent = st.compactAvailable ? "Close" : "Dismiss";
    button.addEventListener("click", () => {
        if (st.compactAvailable) {
            st.expanded = false;
            wrapperElement.classList.add("compact");
            compactButton.setAttribute("aria-expanded", "false");
            compactButton.focus();
            return;
        }

        st.dismissed = true;
        if (st.bodyObserver) try { st.bodyObserver.disconnect(); } catch { }
        if (st.attrObserver) try { st.attrObserver.disconnect(); } catch { }
        host.remove();
    });

    compactButton.addEventListener("click", () => {
        st.expanded = true;
        wrapperElement.classList.remove("compact");
        compactButton.setAttribute("aria-expanded", "true");
        button.focus();
    });

    wrapperElement.appendChild(messageElement);
    wrapperElement.appendChild(button);
    wrapperElement.appendChild(compactButton);
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

export function waitForAnimationFrame() {
    return new Promise(resolve => window.requestAnimationFrame(() => resolve()));
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

export function appendToDocumentHead(element) {
    const head = document.head || document.getElementsByTagName("head")[0];

    if (!head || !element) {
        return null;
    }

    head.appendChild(element);

    return element;
}

export function insertCSSIntoDocumentHead(url) {
    if (!url) {
        return null;
    }

    const existingStylesheet = findStylesheet(url);

    if (existingStylesheet) {
        if (existingStylesheet.getAttribute("href") !== url) {
            existingStylesheet.setAttribute("href", url);
        }

        return existingStylesheet;
    }

    const link = document.createElement("link");

    link.rel = "stylesheet";
    link.setAttribute("href", url);

    return appendToDocumentHead(link);
}

function findStylesheet(url) {
    const stylesheetUrl = createUrl(url);

    return Array.from(document.querySelectorAll('link[rel="stylesheet"]'))
        .find(link => isStylesheetMatch(link, url, stylesheetUrl));
}

function isStylesheetMatch(link, url, stylesheetUrl) {
    if (!link) {
        return false;
    }

    const href = link.getAttribute("href");

    if (href === url || link.href === stylesheetUrl.href) {
        return true;
    }

    const linkUrl = createUrl(link.href || href);

    return linkUrl.origin === stylesheetUrl.origin
        && linkUrl.pathname === stylesheetUrl.pathname;
}

function createUrl(url) {
    try {
        return new URL(url, document.baseURI);
    }
    catch {
        return new URL("about:blank");
    }
}

export function isSystemDarkMode() {
    return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
}