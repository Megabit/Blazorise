let sectionResize;
const listenerOptions = { capture: true, passive: false };
const treeDragImageSuppressors = new WeakMap();

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