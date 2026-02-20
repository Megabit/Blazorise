namespace Blazorise;

/// <summary>
/// Component classes for <see cref="PageProgress"/>.
/// </summary>
public sealed record class PageProgressClasses : ComponentClasses
{
    /// <summary>
    /// Targets the indicator element.
    /// </summary>
    public string Indicator { get; init; }
}

/// <summary>
/// Component styles for <see cref="PageProgress"/>.
/// </summary>
public sealed record class PageProgressStyles : ComponentStyles
{
    /// <summary>
    /// Targets the indicator element.
    /// </summary>
    public string Indicator { get; init; }
}