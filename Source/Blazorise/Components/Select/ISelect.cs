namespace Blazorise;

/// <summary>
/// Defines methods for managing selectable items and checking their values.
/// </summary>
public interface ISelect
{
    /// <summary>
    /// Adds a selectable item to the collection.
    /// </summary>
    /// <param name="selectItem">The item to add to the selection.</param>
    void AddSelectItem( ISelectItem selectItem );

    /// <summary>
    /// Removes a selectable item from the collection.
    /// </summary>
    /// <param name="selectItem">The item to remove from the selection.</param>
    void RemoveSelectItem( ISelectItem selectItem );

    /// <summary>
    /// Checks if the collection contains a specified value.
    /// </summary>
    /// <param name="value">The value to check for in the selection.</param>
    /// <returns><c>true</c> if the collection contains the value; otherwise, <c>false</c>.</returns>
    bool ContainsValue( object value );
}
