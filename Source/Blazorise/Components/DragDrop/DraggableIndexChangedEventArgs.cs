#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Defines the change drag&amp;drop transaction index.
/// </summary>
public class DraggableIndexChangedEventArgs : EventArgs
{
    /// <summary>
    /// A default <see cref="DraggableIndexChangedEventArgs"/> constructor.
    /// </summary>
    /// <param name="zoneName">Name of the current zone.</param>
    /// <param name="oldZoneName">Name of the previous zone.</param>
    /// <param name="index">Index if the dragable item.</param>
    public DraggableIndexChangedEventArgs( string zoneName, string oldZoneName, int index )
    {
        ZoneName = zoneName;
        OldZoneName = oldZoneName;
        Index = index;
    }

    /// <summary>
    /// Gets the current zone name.
    /// </summary>
    public string ZoneName { get; }

    /// <summary>
    /// Gets the previous zone name.
    /// </summary>
    public string OldZoneName { get; }

    /// <summary>
    /// Gets the index if the dragable item.
    /// </summary>
    public int Index { get; }
}