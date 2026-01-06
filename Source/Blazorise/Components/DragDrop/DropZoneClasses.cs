namespace Blazorise;

/// <summary>
/// Component classes for <see cref="DropZone{TItem}"/>.
/// </summary>
public sealed record class DropZoneClasses : ComponentClasses
{
    /// <summary>
    /// Targets placeholder elements in the drop zone.
    /// </summary>
    public string Placeholder { get; init; }
}

/// <summary>
/// Component styles for <see cref="DropZone{TItem}"/>.
/// </summary>
public sealed record class DropZoneStyles : ComponentStyles
{
    /// <summary>
    /// Targets placeholder elements in the drop zone.
    /// </summary>
    public string Placeholder { get; init; }
}