let sectionResize;
const listenerOptions = { capture: true, passive: false };
const designerKeyboardShortcuts = new WeakMap();
let activeDesignerKeyboardShortcut;
const treeDragImageSuppressors = new WeakMap();
const textTokenEditors = new WeakMap();

const designerControlShortcuts = {
    x: "Cut",
    c: "Copy",
    v: "Paste",
    z: "Undo",
    y: "Redo",
};

const designerControlShiftShortcuts = {
    z: "Redo",
};

const designerPlainShortcuts = {
    Delete: "Delete",
    F2: "EditText",
    ArrowLeft: "MoveLeft",
    ArrowUp: "MoveUp",
    ArrowRight: "MoveRight",
    ArrowDown: "MoveDown",
};

export function startSectionResize( dotNetReference, startClientY ) {
    stopSectionResize();

    sectionResize = {
        dotNetReference,
        ended: false,
        lastClientY: startClientY ?? 0,
        move: event => {
            const resize = sectionResize;

            if ( !resize || resize.ended ) {
                return;
            }

            event.preventDefault();
            resize.lastClientY = getClientY( event, resize.lastClientY );
            resize.dotNetReference.invokeMethodAsync( "OnDocumentSectionResizeMove", resize.lastClientY );
        },
        end: event => {
            const resize = sectionResize;

            if ( !resize || resize.ended ) {
                return;
            }

            event.preventDefault();
            completeSectionResize( resize, getClientY( event, resize.lastClientY ) );
        },
        cancel: event => cancelSectionResize( event ),
    };

    addSectionResizeListeners( document, sectionResize );
    addSectionResizeListeners( window, sectionResize );
}

export function stopSectionResize() {
    const resize = sectionResize;

    if ( !resize ) {
        return;
    }

    clearSectionResize( resize );
}

export function startDesignerKeyboardShortcuts( element, dotNetReference ) {
    if ( !element || typeof element.contains !== "function" || !dotNetReference ) {
        return;
    }

    stopDesignerKeyboardShortcuts( element );

    const shortcuts = {
        dotNetReference,
        pointerDown: event => {
            if ( element.contains( event.target ) ) {
                activeDesignerKeyboardShortcut = shortcuts;
            }
            else if ( activeDesignerKeyboardShortcut === shortcuts ) {
                activeDesignerKeyboardShortcut = null;
            }
        },
        focusIn: event => {
            if ( element.contains( event.target ) ) {
                activeDesignerKeyboardShortcut = shortcuts;
            }
        },
        keyDown: event => {
            if ( activeDesignerKeyboardShortcut !== shortcuts || shouldIgnoreDesignerShortcut( event ) ) {
                return;
            }

            const shortcut = resolveDesignerShortcut( event );

            if ( !shortcut ) {
                return;
            }

            event.preventDefault();
            event.stopPropagation();
            shortcuts.dotNetReference.invokeMethodAsync( "OnDesignerShortcut", shortcut );
        },
    };

    document.addEventListener( "pointerdown", shortcuts.pointerDown, true );
    document.addEventListener( "mousedown", shortcuts.pointerDown, true );
    document.addEventListener( "focusin", shortcuts.focusIn, true );
    document.addEventListener( "keydown", shortcuts.keyDown, true );
    designerKeyboardShortcuts.set( element, shortcuts );
}

export function stopDesignerKeyboardShortcuts( element ) {
    if ( !element ) {
        return;
    }

    const shortcuts = designerKeyboardShortcuts.get( element );

    if ( !shortcuts ) {
        return;
    }

    document.removeEventListener( "pointerdown", shortcuts.pointerDown, true );
    document.removeEventListener( "mousedown", shortcuts.pointerDown, true );
    document.removeEventListener( "focusin", shortcuts.focusIn, true );
    document.removeEventListener( "keydown", shortcuts.keyDown, true );

    if ( activeDesignerKeyboardShortcut === shortcuts ) {
        activeDesignerKeyboardShortcut = null;
    }

    designerKeyboardShortcuts.delete( element );
}

export function getElementOffset( element, clientX, clientY ) {
    if ( !element || typeof element.getBoundingClientRect !== "function" ) {
        return [0, 0];
    }

    const rectangle = element.getBoundingClientRect();

    return [
        Math.max( 0, ( clientX ?? 0 ) - rectangle.left ),
        Math.max( 0, ( clientY ?? 0 ) - rectangle.top ),
    ];
}

export function suppressTreeNativeDragImage( element ) {
    if ( !element || typeof element.addEventListener !== "function" ) {
        return;
    }

    clearTreeNativeDragImage( element );

    const suppressor = {
        dragImage: createTransparentDragImage(),
        dragStart: event => {
            if ( !event.target?.closest?.( ".b-report-treeview-row.draggable" ) || !event.dataTransfer?.setDragImage ) {
                return;
            }

            event.dataTransfer.setDragImage( suppressor.dragImage, 0, 0 );
        },
    };

    element.addEventListener( "dragstart", suppressor.dragStart, true );
    treeDragImageSuppressors.set( element, suppressor );
}

export function clearTreeNativeDragImage( element ) {
    if ( !element || typeof element.removeEventListener !== "function" ) {
        return;
    }

    const suppressor = treeDragImageSuppressors.get( element );

    if ( !suppressor ) {
        return;
    }

    element.removeEventListener( "dragstart", suppressor.dragStart, true );
    treeDragImageSuppressors.delete( element );
}

export function protectTextExpressionTokens( element ) {
    if ( !element || typeof element.addEventListener !== "function" ) {
        return;
    }

    clearTextExpressionTokenProtection( element );

    const editor = {
        keyDown: event => handleTextExpressionTokenKeyDown( element, event ),
        replace: () => prepareTextExpressionTokenReplacement( element ),
    };

    element.addEventListener( "keydown", editor.keyDown, true );
    element.addEventListener( "paste", editor.replace, true );
    element.addEventListener( "cut", editor.replace, true );
    textTokenEditors.set( element, editor );
}

export function clearTextExpressionTokenProtection( element ) {
    if ( !element || typeof element.removeEventListener !== "function" ) {
        return;
    }

    const editor = textTokenEditors.get( element );

    if ( !editor ) {
        return;
    }

    element.removeEventListener( "keydown", editor.keyDown, true );
    element.removeEventListener( "paste", editor.replace, true );
    element.removeEventListener( "cut", editor.replace, true );
    textTokenEditors.delete( element );
}

function clearSectionResize( resize ) {
    removeSectionResizeListeners( document, resize );
    removeSectionResizeListeners( window, resize );

    if ( sectionResize === resize ) {
        sectionResize = null;
    }
}

function addSectionResizeListeners( target, resize ) {
    target.addEventListener( "pointermove", resize.move, listenerOptions );
    target.addEventListener( "pointerup", resize.end, listenerOptions );
    target.addEventListener( "pointercancel", resize.cancel, listenerOptions );
    target.addEventListener( "mousemove", resize.move, listenerOptions );
    target.addEventListener( "mouseup", resize.end, listenerOptions );
    target.addEventListener( "touchmove", resize.move, listenerOptions );
    target.addEventListener( "touchend", resize.end, listenerOptions );
    target.addEventListener( "touchcancel", resize.cancel, listenerOptions );
    target.addEventListener( "dragend", resize.end, listenerOptions );
    target.addEventListener( "blur", resize.cancel, listenerOptions );
}

function removeSectionResizeListeners( target, resize ) {
    target.removeEventListener( "pointermove", resize.move, true );
    target.removeEventListener( "pointerup", resize.end, true );
    target.removeEventListener( "pointercancel", resize.cancel, true );
    target.removeEventListener( "mousemove", resize.move, true );
    target.removeEventListener( "mouseup", resize.end, true );
    target.removeEventListener( "touchmove", resize.move, true );
    target.removeEventListener( "touchend", resize.end, true );
    target.removeEventListener( "touchcancel", resize.cancel, true );
    target.removeEventListener( "dragend", resize.end, true );
    target.removeEventListener( "blur", resize.cancel, true );
}

async function completeSectionResize( resize, clientY ) {
    if ( !resize || resize.ended ) {
        return;
    }

    resize.ended = true;
    clearSectionResize( resize );

    await resize.dotNetReference.invokeMethodAsync( "OnDocumentSectionResizeEnd", clientY );
}

async function cancelSectionResize( event ) {
    if ( event ) {
        event.preventDefault();
    }

    const resize = sectionResize;

    if ( !resize || resize.ended ) {
        return;
    }

    resize.ended = true;
    clearSectionResize( resize );

    await resize.dotNetReference.invokeMethodAsync( "OnDocumentSectionResizeCancel" );
}

function getClientY( event, fallback ) {
    if ( typeof event?.clientY === "number" ) {
        return event.clientY;
    }

    const touch = event?.changedTouches?.[0] ?? event?.touches?.[0];

    return typeof touch?.clientY === "number"
        ? touch.clientY
        : fallback;
}

function createTransparentDragImage() {
    const canvas = document.createElement( "canvas" );
    canvas.width = 1;
    canvas.height = 1;

    return canvas;
}

function resolveDesignerShortcut( event ) {
    if ( isDesignerControlShortcut( event ) ) {
        return designerControlShortcuts[event.key?.toLowerCase?.()] ?? null;
    }

    if ( isDesignerControlShiftShortcut( event ) ) {
        return designerControlShiftShortcuts[event.key?.toLowerCase?.()] ?? null;
    }

    return isDesignerPlainShortcut( event )
        ? designerPlainShortcuts[event.key] ?? null
        : null;
}

function isDesignerControlShortcut( event ) {
    return event.ctrlKey && !event.shiftKey && !event.altKey && !event.metaKey;
}

function isDesignerControlShiftShortcut( event ) {
    return event.ctrlKey && event.shiftKey && !event.altKey && !event.metaKey;
}

function isDesignerPlainShortcut( event ) {
    return !event.ctrlKey && !event.shiftKey && !event.altKey && !event.metaKey;
}

function shouldIgnoreDesignerShortcut( event ) {
    const target = event.target;

    if ( !target ) {
        return false;
    }

    const tagName = target.tagName?.toLowerCase?.();

    return tagName === "input"
        || tagName === "textarea"
        || tagName === "select"
        || target.isContentEditable
        || !!target.closest?.( ".b-report-element-text-editor" );
}

function handleTextExpressionTokenKeyDown( element, event ) {
    if ( event.key === "Backspace" || event.key === "Delete" ) {
        handleTextExpressionTokenDelete( element, event );
        return;
    }

    if ( isTextInputKey( event ) ) {
        prepareTextExpressionTokenReplacement( element );
    }
}

function handleTextExpressionTokenDelete( element, event ) {
    const selectionStart = element.selectionStart;
    const selectionEnd = element.selectionEnd;

    if ( typeof selectionStart !== "number" || typeof selectionEnd !== "number" ) {
        return;
    }

    const range = getProtectedTokenDeletionRange( element.value ?? "", selectionStart, selectionEnd, event.key );

    if ( !range ) {
        return;
    }

    event.preventDefault();
    replaceTextRange( element, range.start, range.end, "" );
}

function prepareTextExpressionTokenReplacement( element ) {
    const selectionStart = element.selectionStart;
    const selectionEnd = element.selectionEnd;

    if ( typeof selectionStart !== "number" || typeof selectionEnd !== "number" ) {
        return;
    }

    const range = getProtectedTokenReplacementRange( element.value ?? "", selectionStart, selectionEnd );

    if ( range ) {
        element.setSelectionRange( range.start, range.end );
    }
}

function getProtectedTokenDeletionRange( value, selectionStart, selectionEnd, key ) {
    const tokenRanges = getExpressionTokenRanges( value );

    if ( tokenRanges.length === 0 ) {
        return null;
    }

    if ( selectionStart !== selectionEnd ) {
        let start = selectionStart;
        let end = selectionEnd;
        let expanded = false;

        for ( const tokenRange of tokenRanges ) {
            if ( tokenRange.end <= selectionStart || tokenRange.start >= selectionEnd ) {
                continue;
            }

            start = Math.min( start, tokenRange.start );
            end = Math.max( end, tokenRange.end );
            expanded = true;
        }

        return expanded ? { start, end } : null;
    }

    for ( const tokenRange of tokenRanges ) {
        if ( key === "Backspace" && selectionStart > tokenRange.start && selectionStart <= tokenRange.end ) {
            return tokenRange;
        }

        if ( key === "Delete" && selectionStart >= tokenRange.start && selectionStart < tokenRange.end ) {
            return tokenRange;
        }
    }

    return null;
}

function getProtectedTokenReplacementRange( value, selectionStart, selectionEnd ) {
    const tokenRanges = getExpressionTokenRanges( value );

    if ( tokenRanges.length === 0 ) {
        return null;
    }

    if ( selectionStart !== selectionEnd ) {
        let start = selectionStart;
        let end = selectionEnd;
        let expanded = false;

        for ( const tokenRange of tokenRanges ) {
            if ( tokenRange.end <= selectionStart || tokenRange.start >= selectionEnd ) {
                continue;
            }

            start = Math.min( start, tokenRange.start );
            end = Math.max( end, tokenRange.end );
            expanded = true;
        }

        return expanded ? { start, end } : null;
    }

    for ( const tokenRange of tokenRanges ) {
        if ( selectionStart > tokenRange.start && selectionStart < tokenRange.end ) {
            return tokenRange;
        }
    }

    return null;
}

function isTextInputKey( event ) {
    return !event.ctrlKey
        && !event.metaKey
        && !event.altKey
        && event.key?.length === 1;
}

function getExpressionTokenRanges( value ) {
    const ranges = [];
    const expressionRegex = /\{[^{}\r\n]+\}/g;
    let match;

    while ( ( match = expressionRegex.exec( value ) ) !== null ) {
        ranges.push( {
            start: match.index,
            end: match.index + match[0].length,
        } );
    }

    return ranges;
}

function replaceTextRange( element, start, end, replacement ) {
    const value = element.value ?? "";
    element.value = value.substring( 0, start ) + replacement + value.substring( end );
    element.setSelectionRange( start + replacement.length, start + replacement.length );
    dispatchInputEvent( element );
}

function dispatchInputEvent( element ) {
    let event;

    try {
        event = new InputEvent( "input", { bubbles: true, inputType: "deleteContentBackward" } );
    }
    catch {
        event = new Event( "input", { bubbles: true } );
    }

    element.dispatchEvent( event );
}