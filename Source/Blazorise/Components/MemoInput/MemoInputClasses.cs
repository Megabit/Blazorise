namespace Blazorise;

/// <summary>
/// Component classes for <see cref="MemoInput"/>.
/// </summary>
public sealed record class MemoInputClasses : ComponentClasses
{
    /// <summary>
    /// Targets the input wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}

/// <summary>
/// Component styles for <see cref="MemoInput"/>.
/// </summary>
public sealed record class MemoInputStyles : ComponentStyles
{
    /// <summary>
    /// Targets the input wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}