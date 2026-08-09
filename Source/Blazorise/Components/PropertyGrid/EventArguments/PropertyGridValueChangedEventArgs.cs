#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Supplies information about a property value change.
/// </summary>
public sealed class PropertyGridValueChangedEventArgs : EventArgs
{
    #region Constructors

    /// <summary>
    /// Initializes value change event data.
    /// </summary>
    public PropertyGridValueChangedEventArgs( PropertyGridProperty property, object value )
    {
        Property = property;
        Value = value;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the new property value as the requested type.
    /// </summary>
    public TValue GetValue<TValue>() => (TValue)Value;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the changed property definition.
    /// </summary>
    public PropertyGridProperty Property { get; }

    /// <summary>
    /// Gets the changed property key.
    /// </summary>
    public string PropertyKey => Property.Key;

    /// <summary>
    /// Gets the new property value.
    /// </summary>
    public object Value { get; }

    #endregion
}