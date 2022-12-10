#region Using directives
#endregion


namespace Blazorise;

/// <summary>
/// Interface for the components based on column sizes.
/// </summary>
public interface IRowableContext
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
    /// <remarks>
    /// The used space will reset only if the column is the first columnn in a row(has a zero index). This should work
    /// most of the time. But, in some rare cases when columns are (re)created dynamicaly(for example if it's
    /// placed behind an if statement) then the column would have a highest index once it is recreated. For those cases
    /// it is advised to developers to use the <see cref="BaseComponent.Display"/> parameter to show or hide the column.
    /// </remarks>
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