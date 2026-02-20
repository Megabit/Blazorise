namespace Blazorise;

/// <summary>
/// Component classes for <see cref="Offcanvas"/>.
/// </summary>
public sealed record class OffcanvasClasses : ComponentClasses
{
    /// <summary>
    /// Targets the offcanvas backdrop element.
    /// </summary>
    public string Backdrop { get; init; }
}

/// <summary>
/// Component styles for <see cref="Offcanvas"/>.
/// </summary>
public sealed record class OffcanvasStyles : ComponentStyles
{
    /// <summary>
    /// Targets the offcanvas backdrop element.
    /// </summary>
    public string Backdrop { get; init; }
}