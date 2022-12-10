#region Using directives
#endregion

namespace Blazorise;

/// <summary>
/// Interface for the components based on column sizes.
/// </summary>
public interface IRowableComponent
{
    /// <summary>
    /// Notifies the row component that one of its child component is a column and that it is initialized.
    /// </summary>
    /// <param name="column">Column that is initialized.</param>
    void NotifyColumnableInitialized( IColumnableComponent column );

    /// <summary>
    /// Notifies the row component that one of its child component is a column and that it is disposed.
    /// </summary>
    /// <param name="column">Column that is disposed.</param>
    void NotifyColumnableRemoved( IColumnableComponent column );

    /// <summary>
    /// Forces the row to start calculating used space from start.
    /// </summary>
    /// <param name="column">Column that is requested the calculation.</param>
    void ResetUsedSpace( IColumnableComponent column );

    /// <summary>
    /// Increases the used space by the specified amount.
    /// </summary>
    /// <param name="space">Space to increase.</param>
    void IncreaseUsedSpace( int space );

    /// <summary>
    /// Gets the total space used by the columns placed inside of the row component.
    /// </summary>
    int TotalUsedSpace { get; }
}
