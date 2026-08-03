import { getRequiredElement, registerDisconnectCleanup, unregisterDisconnectCleanup } from "./utilities.js?v=2.2.2.0";
import { createFloatingUiAutoUpdate, createFloatingUiPointAutoUpdate } from './floatingUi.js?v=2.2.2.0';

const _instances = [];
const menuItemSelector = '[data-context-menu-item="true"]';
const submenuTriggerSelector = '[data-context-menu-submenu-trigger="true"]';
const keyboardNavigationAttribute = 'data-context-menu-keyboard-navigation';
const typeaheadResetDelay = 500;

export function initialize(element, elementId, menuElementId, clientX, clientY, contextElementSelector, options) {
    element = getRequiredElement(element, elementId);

    if (!element)
        return;

    const menuElement = menuElementId
        ? document.getElementById(menuElementId)
        : element.querySelector('[role="menu"]');

    if (!menuElement)
        return;

    const previousInstance = _instances[elementId];
    const selectedContextElement = findContextElement(contextElementSelector);
    const activeElement = document.activeElement;
    const restoreFocusElement = activeElement
        && activeElement !== document.body
        && !menuElement.contains(activeElement)
        ? activeElement
        : selectedContextElement ?? previousInstance?.restoreFocusElement;

    destroy(null, elementId);

    const contextElement = selectedContextElement ?? element;

    const positionCleanupFunction = Number.isFinite(clientX) && Number.isFinite(clientY)
        ? createFloatingUiPointAutoUpdate(clientX, clientY, contextElement, menuElement, options)
        : createFloatingUiAutoUpdate(contextElement, menuElement, options);
    const navigationCleanupFunction = initializeMenuNavigation(menuElement);

    _instances[elementId] = {
        menuElement,
        restoreFocusElement,
        cleanupFunction() {
            positionCleanupFunction?.();
            navigationCleanupFunction?.();
        },
        disconnectCleanupId: registerDisconnectCleanup(element, () => destroy(null, elementId, false))
    };
}

export function restoreFocus(element, elementId) {
    const instance = _instances[elementId];

    if (!instance?.restoreFocusElement?.isConnected)
        return;

    const activeElement = document.activeElement;

    if (activeElement
        && activeElement !== document.body
        && activeElement !== instance.menuElement
        && !instance.menuElement.contains(activeElement)) {
        return;
    }

    focusElement(instance.restoreFocusElement);
}

export function destroy(element, elementId, unregisterCleanup = true) {
    const instances = _instances || {};
    const instance = instances[elementId];

    if (instance) {
        if (unregisterCleanup) {
            unregisterDisconnectCleanup(instance.disconnectCleanupId);
        }

        if (instance.cleanupFunction) {
            instance.cleanupFunction();
        }

        delete instances[elementId];
    }
}

function findContextElement(contextElementSelector) {
    if (!contextElementSelector) {
        return null;
    }

    try {
        return document.querySelector(contextElementSelector);
    }
    catch {
        return null;
    }
}

function initializeMenuNavigation(menuElement) {
    let typeaheadText = '';
    let typeaheadTimer;
    let submenuObserver;

    menuElement.setAttribute(keyboardNavigationAttribute, 'false');

    const clearSubmenuFocus = () => {
        submenuObserver?.disconnect();
        submenuObserver = null;
    };

    const focusSubmenu = trigger => {
        clearSubmenuFocus();

        const tryFocus = () => {
            if (trigger.getAttribute('aria-expanded') !== 'true')
                return false;

            const submenuId = trigger.getAttribute('aria-controls');
            const submenu = submenuId ? document.getElementById(submenuId) : null;

            if (!submenu || !menuElement.contains(submenu))
                return false;

            const items = getMenuItems(submenu);

            if (items.length > 0) {
                setActiveItem(submenu, items[0]);
            }
            else {
                focusElement(submenu);
            }

            clearSubmenuFocus();

            return true;
        };

        if (tryFocus())
            return;

        submenuObserver = new MutationObserver(tryFocus);
        submenuObserver.observe(trigger, { attributes: true, attributeFilter: ['aria-controls', 'aria-expanded'] });
    };

    const closeSubmenu = submenu => {
        const trigger = findSubmenuTrigger(menuElement, submenu);

        if (!trigger)
            return false;

        if (trigger.getAttribute('aria-expanded') === 'true')
            toggleSubmenu(trigger);

        const parentMenu = trigger.closest('[role="menu"]');

        if (parentMenu)
            setActiveItem(parentMenu, trigger);
        else
            focusElement(trigger);

        return true;
    };

    const onFocusIn = event => {
        const item = event.target instanceof Element
            ? event.target.closest(menuItemSelector)
            : null;
        const menu = item?.closest('[role="menu"]');

        if (item && menu && menuElement.contains(menu) && !isDisabled(item))
            setActiveItem(menu, item, false);
    };

    const onPointerDown = () => menuElement.setAttribute(keyboardNavigationAttribute, 'false');

    const onKeyDown = event => {
        if (!(event.target instanceof Element))
            return;

        const menu = event.target.closest('[role="menu"]');

        if (!menu || (menu !== menuElement && !menuElement.contains(menu)))
            return;

        if (isMenuNavigationKey(event))
            menuElement.setAttribute(keyboardNavigationAttribute, 'true');

        const items = getMenuItems(menu);

        if (items.length === 0)
            return;

        const currentItem = getCurrentItem(event.target, menu, items);
        const currentIndex = items.indexOf(currentItem);
        const direction = getComputedStyle(menu).direction;
        const openSubmenuKey = direction === 'rtl' ? 'ArrowLeft' : 'ArrowRight';
        const closeSubmenuKey = direction === 'rtl' ? 'ArrowRight' : 'ArrowLeft';
        let nextItem;

        if (event.key === 'ArrowDown') {
            nextItem = items[(currentIndex + 1) % items.length];
        }
        else if (event.key === 'ArrowUp') {
            nextItem = currentIndex < 0
                ? items[items.length - 1]
                : items[(currentIndex - 1 + items.length) % items.length];
        }
        else if (event.key === 'Home') {
            nextItem = items[0];
        }
        else if (event.key === 'End') {
            nextItem = items[items.length - 1];
        }
        else if (event.key === openSubmenuKey && currentItem?.matches(submenuTriggerSelector)) {
            event.preventDefault();
            event.stopPropagation();

            if (currentItem.getAttribute('aria-expanded') !== 'true')
                toggleSubmenu(currentItem);

            focusSubmenu(currentItem);

            return;
        }
        else if ((event.key === closeSubmenuKey || event.key === 'Escape') && menu !== menuElement) {
            if (!closeSubmenu(menu))
                return;

            event.preventDefault();
            event.stopPropagation();

            return;
        }
        else if ((event.key === 'Enter' || event.key === 'NumpadEnter' || event.key === ' ')
            && currentItem?.matches('a')) {
            event.preventDefault();
            event.stopPropagation();
            activateElement(currentItem);

            return;
        }
        else if (isTypeaheadKey(event)) {
            clearTimeout(typeaheadTimer);
            typeaheadText += event.key.toLocaleLowerCase();
            typeaheadTimer = setTimeout(() => typeaheadText = '', typeaheadResetDelay);

            const typeaheadCharacters = Array.from(typeaheadText);
            const repeatedCharacter = typeaheadCharacters.every(character => character === typeaheadCharacters[0]);
            const searchText = repeatedCharacter ? typeaheadCharacters[0] : typeaheadText;
            nextItem = findTypeaheadItem(items, currentIndex, searchText);

            if (!nextItem)
                return;
        }
        else {
            return;
        }

        event.preventDefault();
        event.stopPropagation();
        setActiveItem(menu, nextItem);
    };

    menuElement.addEventListener('focusin', onFocusIn);
    menuElement.addEventListener('pointerdown', onPointerDown);
    menuElement.addEventListener('keydown', onKeyDown);

    for (const item of menuElement.querySelectorAll(menuItemSelector))
        item.tabIndex = -1;

    focusElement(menuElement);

    return () => {
        clearTimeout(typeaheadTimer);
        clearSubmenuFocus();
        menuElement.removeEventListener('focusin', onFocusIn);
        menuElement.removeEventListener('pointerdown', onPointerDown);
        menuElement.removeEventListener('keydown', onKeyDown);
    };
}

function getMenuItems(menu) {
    return Array.from(menu.querySelectorAll(menuItemSelector))
        .filter(item => item.closest('[role="menu"]') === menu && !isDisabled(item));
}

function getCurrentItem(eventTarget, menu, items) {
    const targetItem = eventTarget.closest(menuItemSelector);

    if (targetItem && targetItem.closest('[role="menu"]') === menu && items.includes(targetItem))
        return targetItem;

    const focusedItem = document.activeElement instanceof Element
        ? document.activeElement.closest(menuItemSelector)
        : null;

    if (focusedItem && focusedItem.closest('[role="menu"]') === menu && items.includes(focusedItem))
        return focusedItem;

    return items.find(item => item.tabIndex === 0) ?? null;
}

function setActiveItem(menu, activeItem, moveFocus = true) {
    for (const item of menu.querySelectorAll(menuItemSelector)) {
        if (item.closest('[role="menu"]') === menu)
            item.tabIndex = item === activeItem ? 0 : -1;
    }

    if (moveFocus)
        focusElement(activeItem);
}

function findTypeaheadItem(items, currentIndex, searchText) {
    for (let offset = 1; offset <= items.length; offset++) {
        const item = items[(currentIndex + offset) % items.length];
        const itemText = (item.getAttribute('aria-label') ?? item.textContent ?? '')
            .trim()
            .replace(/\s+/g, ' ')
            .toLocaleLowerCase();

        if (itemText.startsWith(searchText))
            return item;
    }

    return null;
}

function findSubmenuTrigger(menuElement, submenu) {
    if (!submenu.id)
        return null;

    return Array.from(menuElement.querySelectorAll(submenuTriggerSelector))
        .find(trigger => trigger.getAttribute('aria-controls') === submenu.id);
}

function isDisabled(item) {
    return item.hasAttribute('disabled') || item.getAttribute('aria-disabled') === 'true';
}

function isTypeaheadKey(event) {
    return event.key.length === 1
        && event.key !== ' '
        && !event.altKey
        && !event.ctrlKey
        && !event.metaKey;
}

function isMenuNavigationKey(event) {
    return event.key === 'ArrowDown'
        || event.key === 'ArrowUp'
        || event.key === 'ArrowLeft'
        || event.key === 'ArrowRight'
        || event.key === 'Home'
        || event.key === 'End'
        || isTypeaheadKey(event);
}

function activateElement(element) {
    element.dispatchEvent(new MouseEvent('click', {
        bubbles: true,
        cancelable: true,
        detail: 1,
        view: window
    }));
}

function toggleSubmenu(trigger) {
    trigger.dispatchEvent(new KeyboardEvent('keydown', {
        bubbles: true,
        cancelable: true,
        code: 'Enter',
        key: 'Enter'
    }));
}

function focusElement(element) {
    if (!element?.isConnected || typeof element.focus !== 'function')
        return;

    const shouldRestoreTabIndex = element.tabIndex < 0 && !element.hasAttribute('tabindex');

    if (shouldRestoreTabIndex)
        element.setAttribute('tabindex', '-1');

    element.focus({ preventScroll: true });

    if (shouldRestoreTabIndex)
        element.removeAttribute('tabindex');
}