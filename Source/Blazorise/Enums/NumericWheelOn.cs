namespace Blazorise;

/// <summary>
/// Defines when the wheel event will increment or decrement the element value.
/// </summary>
public enum NumericWheelOn
{
    /// <summary>
    /// When `wheelOn` is set to `'focus'`, you can use the 'Shift' modifier key while using the mouse wheel in order to temporarily activate the increment/decrement feature even if the element is not focused.
    /// </summary>
    Focus,

    /// <summary>
    /// When `wheelOn` is set to `'hover'`, you can use the 'Shift' modifier key while using the mouse wheel in order to temporarily disable the increment/decrement feature even if the element is not hovered.
    /// </summary>
    Hover,
}