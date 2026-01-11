namespace Blazorise;

/// <summary>
/// Component classes for <see cref="TableRowGroup"/>.
/// </summary>
public sealed record class TableRowGroupClasses : ComponentClasses
{
    /// <summary>
    /// Targets grouped row cell elements.
    /// </summary>
    public string RowCell { get; init; }

    /// <summary>
    /// Targets grouped row indent cell elements.
    /// </summary>
    public string RowIndentCell { get; init; }
}

/// <summary>
/// Component styles for <see cref="TableRowGroup"/>.
/// </summary>
public sealed record class TableRowGroupStyles : ComponentStyles
{
    /// <summary>
    /// Targets grouped row cell elements.
    /// </summary>
    public string RowCell { get; init; }

    /// <summary>
    /// Targets grouped row indent cell elements.
    /// </summary>
    public string RowIndentCell { get; init; }
}