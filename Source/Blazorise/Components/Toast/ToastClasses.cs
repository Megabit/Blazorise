namespace Blazorise;

/// <summary>
/// Component classes for <see cref="Toast"/>.
/// </summary>
public sealed record class ToastClasses : ComponentClasses
{
    /// <summary>
    /// Targets the wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}

/// <summary>
/// Component styles for <see cref="Toast"/>.
/// </summary>
public sealed record class ToastStyles : ComponentStyles
{
    /// <summary>
    /// Targets the wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}