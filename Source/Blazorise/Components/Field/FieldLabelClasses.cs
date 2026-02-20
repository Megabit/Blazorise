namespace Blazorise;

/// <summary>
/// Component classes for <see cref="FieldLabel"/>.
/// </summary>
public sealed record class FieldLabelClasses : ComponentClasses
{
    /// <summary>
    /// Targets the label container element.
    /// </summary>
    public string Container { get; init; }
}

/// <summary>
/// Component styles for <see cref="FieldLabel"/>.
/// </summary>
public sealed record class FieldLabelStyles : ComponentStyles
{
    /// <summary>
    /// Targets the label container element.
    /// </summary>
    public string Container { get; init; }
}