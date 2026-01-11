namespace Blazorise;

/// <summary>
/// Component classes for <see cref="ValidationSummary"/>.
/// </summary>
public sealed record class ValidationSummaryClasses : ComponentClasses
{
    /// <summary>
    /// Targets validation error list item elements.
    /// </summary>
    public string Error { get; init; }
}

/// <summary>
/// Component styles for <see cref="ValidationSummary"/>.
/// </summary>
public sealed record class ValidationSummaryStyles : ComponentStyles
{
    /// <summary>
    /// Targets validation error list item elements.
    /// </summary>
    public string Error { get; init; }
}