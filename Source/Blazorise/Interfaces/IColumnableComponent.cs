#region Using directives
#endregion

namespace Blazorise;

/// <summary>
/// Interface for the components based on column sizes.
/// </summary>
public interface IColumnableComponent
{
    /// <summary>
    /// Defines the sizing rules for the column.
    /// </summary>
    IFluentColumn ColumnSize { get; }
}
