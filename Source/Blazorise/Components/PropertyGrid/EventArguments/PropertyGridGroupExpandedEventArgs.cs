#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Supplies information about a property group expansion change.
/// </summary>
public sealed class PropertyGridGroupExpandedEventArgs : EventArgs
{
    #region Constructors

    /// <summary>
    /// Initializes property group expansion event data.
    /// </summary>
    public PropertyGridGroupExpandedEventArgs( PropertyGridGroupDefinition group, bool expanded )
    {
        Group = group;
        Expanded = expanded;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the changed property group.
    /// </summary>
    public PropertyGridGroupDefinition Group { get; }

    /// <summary>
    /// Gets whether the property group is expanded.
    /// </summary>
    public bool Expanded { get; }

    #endregion
}