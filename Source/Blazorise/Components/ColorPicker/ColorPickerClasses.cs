namespace Blazorise;

/// <summary>
/// Component classes for <see cref="ColorPicker"/>.
/// </summary>
public sealed record class ColorPickerClasses : ComponentClasses
{
    /// <summary>
    /// Targets the picker wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}

/// <summary>
/// Component styles for <see cref="ColorPicker"/>.
/// </summary>
public sealed record class ColorPickerStyles : ComponentStyles
{
    /// <summary>
    /// Targets the picker wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}