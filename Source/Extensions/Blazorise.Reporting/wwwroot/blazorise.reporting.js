let sectionResize;
const listenerOptions = { capture: true, passive: false };

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