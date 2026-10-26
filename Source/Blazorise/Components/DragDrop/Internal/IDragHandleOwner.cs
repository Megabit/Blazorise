namespace Blazorise;

/// <summary>
/// Connects drag handles to their owning item independently of its data type.
/// </summary>
internal interface IDragHandleOwner
{
    /// <summary>
    /// Gets whether the owning item is disabled.
    /// </summary>
    bool Disabled { get; }

    /// <summary>
    /// Registers a handle that restricts where item dragging can begin.
    /// </summary>
    void RegisterHandle( DragHandle handle );

    /// <summary>
    /// Removes a handle from the item.
    /// </summary>
    void UnregisterHandle( DragHandle handle );
}