namespace Blazorise;

/// <summary>
/// Component classes for <see cref="TextInput"/>.
/// </summary>
public sealed record class TextInputClasses : ComponentClasses
{
    /// <summary>
    /// Targets the input wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}

/// <summary>
/// Component styles for <see cref="TextInput"/>.
/// </summary>
public sealed record class TextInputStyles : ComponentStyles
{
    /// <summary>
    /// Targets the input wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}