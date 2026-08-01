#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Supplies information about a property action.
/// </summary>
public sealed class PropertyGridActionEventArgs : EventArgs
{
    #region Constructors

    /// <summary>
    /// Initializes action event data.
    /// </summary>
    public PropertyGridActionEventArgs( PropertyGridProperty property, PropertyGridAction action )
    {
        Property = property;
        Action = action;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the property definition that owns the action.
    /// </summary>
    public PropertyGridProperty Property { get; }

    /// <summary>
    /// Gets the property key.
    /// </summary>
    public string PropertyKey => Property.Key;

    /// <summary>
    /// Gets the invoked action.
    /// </summary>
    public PropertyGridAction Action { get; }

    #endregion
}