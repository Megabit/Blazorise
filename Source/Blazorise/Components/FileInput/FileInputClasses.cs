namespace Blazorise;

/// <summary>
/// Component classes for <see cref="FileInput"/>.
/// </summary>
public sealed record class FileInputClasses : ComponentClasses
{
    /// <summary>
    /// Targets the input wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}

/// <summary>
/// Component styles for <see cref="FileInput"/>.
/// </summary>
public sealed record class FileInputStyles : ComponentStyles
{
    /// <summary>
    /// Targets the input wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}