namespace Blazorise;

/// <summary>
/// Component classes for <see cref="Table"/>.
/// </summary>
public sealed record class TableClasses : ComponentClasses
{
    /// <summary>
    /// Targets the table container element.
    /// </summary>
    public string Container { get; init; }
}

/// <summary>
/// Component styles for <see cref="Table"/>.
/// </summary>
public sealed record class TableStyles : ComponentStyles
{
    /// <summary>
    /// Targets the table container element.
    /// </summary>
    public string Container { get; init; }
}