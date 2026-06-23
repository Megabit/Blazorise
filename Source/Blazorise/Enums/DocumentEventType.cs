namespace Blazorise;

/// <summary>
/// Defines a single browser document event type observed by <see cref="IDocumentObserver"/>.
/// </summary>
public enum DocumentEventType
{
    /// <summary>
    /// No document event type.
    /// </summary>
    None,

    /// <summary>
    /// The pointerdown document event.
    /// </summary>
    PointerDown,

    /// <summary>
    /// The pointermove document event.
    /// </summary>
    PointerMove,

    /// <summary>
    /// The pointerup document event.
    /// </summary>
    PointerUp,

    /// <summary>
    /// The pointercancel document event.
    /// </summary>
    PointerCancel,

    /// <summary>
    /// The mousedown document event.
    /// </summary>
    MouseDown,

    /// <summary>
    /// The mousemove document event.
    /// </summary>
    MouseMove,

    /// <summary>
    /// The mouseup document event.
    /// </summary>
    MouseUp,

    /// <summary>
    /// The touchmove document event.
    /// </summary>
    TouchMove,

    /// <summary>
    /// The touchend document event.
    /// </summary>
    TouchEnd,

    /// <summary>
    /// The touchcancel document event.
    /// </summary>
    TouchCancel,

    /// <summary>
    /// The click document event.
    /// </summary>
    Click,

    /// <summary>
    /// The dblclick document event.
    /// </summary>
    DoubleClick,

    /// <summary>
    /// The keydown document event.
    /// </summary>
    KeyDown,

    /// <summary>
    /// The keyup document event.
    /// </summary>
    KeyUp,

    /// <summary>
    /// The focusin document event.
    /// </summary>
    FocusIn,

    /// <summary>
    /// The focusout document event.
    /// </summary>
    FocusOut,

    /// <summary>
    /// The dragstart document event.
    /// </summary>
    DragStart,

    /// <summary>
    /// The dragover document event.
    /// </summary>
    DragOver,

    /// <summary>
    /// The dragend document event.
    /// </summary>
    DragEnd,

    /// <summary>
    /// The drop document event.
    /// </summary>
    Drop,

    /// <summary>
    /// The contextmenu document event.
    /// </summary>
    ContextMenu,

    /// <summary>
    /// The blur window event.
    /// </summary>
    Blur,
}
