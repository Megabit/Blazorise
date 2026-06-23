#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Defines browser document event types observed by <see cref="IDocumentObserver"/>.
/// </summary>
[Flags]
public enum DocumentEventTypes
{
    /// <summary>
    /// No document events.
    /// </summary>
    None = 0,

    /// <summary>
    /// The pointerdown document event.
    /// </summary>
    PointerDown = 1 << 0,

    /// <summary>
    /// The pointermove document event.
    /// </summary>
    PointerMove = 1 << 1,

    /// <summary>
    /// The pointerup document event.
    /// </summary>
    PointerUp = 1 << 2,

    /// <summary>
    /// The pointercancel document event.
    /// </summary>
    PointerCancel = 1 << 3,

    /// <summary>
    /// The mousedown document event.
    /// </summary>
    MouseDown = 1 << 4,

    /// <summary>
    /// The mousemove document event.
    /// </summary>
    MouseMove = 1 << 5,

    /// <summary>
    /// The mouseup document event.
    /// </summary>
    MouseUp = 1 << 6,

    /// <summary>
    /// The touchmove document event.
    /// </summary>
    TouchMove = 1 << 7,

    /// <summary>
    /// The touchend document event.
    /// </summary>
    TouchEnd = 1 << 8,

    /// <summary>
    /// The touchcancel document event.
    /// </summary>
    TouchCancel = 1 << 9,

    /// <summary>
    /// The click document event.
    /// </summary>
    Click = 1 << 10,

    /// <summary>
    /// The dblclick document event.
    /// </summary>
    DoubleClick = 1 << 11,

    /// <summary>
    /// The keydown document event.
    /// </summary>
    KeyDown = 1 << 12,

    /// <summary>
    /// The keyup document event.
    /// </summary>
    KeyUp = 1 << 13,

    /// <summary>
    /// The focusin document event.
    /// </summary>
    FocusIn = 1 << 14,

    /// <summary>
    /// The focusout document event.
    /// </summary>
    FocusOut = 1 << 15,

    /// <summary>
    /// The dragstart document event.
    /// </summary>
    DragStart = 1 << 16,

    /// <summary>
    /// The dragover document event.
    /// </summary>
    DragOver = 1 << 17,

    /// <summary>
    /// The dragend document event.
    /// </summary>
    DragEnd = 1 << 18,

    /// <summary>
    /// The drop document event.
    /// </summary>
    Drop = 1 << 19,

    /// <summary>
    /// The contextmenu document event.
    /// </summary>
    ContextMenu = 1 << 20,

    /// <summary>
    /// The blur window event.
    /// </summary>
    Blur = 1 << 21,

    /// <summary>
    /// Pointer events commonly used for drag or resize operations.
    /// </summary>
    PointerDrag = PointerMove | PointerUp | PointerCancel,
}
