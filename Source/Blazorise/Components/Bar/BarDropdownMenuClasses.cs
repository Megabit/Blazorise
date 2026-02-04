namespace Blazorise;

/// <summary>
/// Component classes for <see cref="BarDropdownMenu"/>.
/// </summary>
public sealed record class BarDropdownMenuClasses : ComponentClasses
{
    /// <summary>
    /// Targets the dropdown menu container element.
    /// </summary>
    public string Container { get; init; }
}

/// <summary>
/// Component styles for <see cref="BarDropdownMenu"/>.
/// </summary>
public sealed record class BarDropdownMenuStyles : ComponentStyles
{
    /// <summary>
    /// Targets the dropdown menu container element.
    /// </summary>
    public string Container { get; init; }
}