namespace Blazorise;

/// <summary>
/// Defines the color selection mode of a <see cref="ColorPicker"/>.
/// </summary>
public enum ColorPickerMode
{
    /// <summary>
    /// Selects saturation and brightness in a square, with hue controlled separately.
    /// </summary>
    Square,

    /// <summary>
    /// Selects hue and saturation in a wheel, with brightness controlled separately.
    /// </summary>
    Wheel,
}