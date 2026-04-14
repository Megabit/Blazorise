using System;

namespace Blazorise.Maps;

/// <summary>
/// Provides data for data-bound marker click events.
/// </summary>
/// <typeparam name="TItem">The marker item type.</typeparam>
public class MapMarkerClickedEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Initializes a new data-bound marker click event args instance.
    /// </summary>
    /// <param name="item">The marker item.</param>
    /// <param name="id">The marker id.</param>
    /// <param name="mouseEventArgs">The map mouse event args.</param>
    public MapMarkerClickedEventArgs( TItem item, string id, MapMouseEventArgs mouseEventArgs )
    {
        Item = item;
        Id = id;
        MouseEventArgs = mouseEventArgs;
    }

    /// <summary>
    /// Contains the data item represented by the marker.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Identifies the marker involved in the event.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Contains the originating map mouse interaction.
    /// </summary>
    public MapMouseEventArgs MouseEventArgs { get; }
}