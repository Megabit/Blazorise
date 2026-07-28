using System;

namespace Blazorise;

/// <summary>
/// Supplies information about a property value change.
/// </summary>
public sealed class PropertyGridValueChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes value change event data.
    /// </summary>
    public PropertyGridValueChangedEventArgs( PropertyGridProperty property, object value )
    {
        Property = property;
        Value = value;
    }

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

    /// <summary>
    /// Gets the new property value as the requested type.
    /// </summary>
    public TValue GetValue<TValue>() => (TValue)Value;
}

/// <summary>
/// Supplies information about a property action.
/// </summary>
public sealed class PropertyGridActionEventArgs : EventArgs
{
    /// <summary>
    /// Initializes action event data.
    /// </summary>
    public PropertyGridActionEventArgs( PropertyGridProperty property, PropertyGridAction action )
    {
        Property = property;
        Action = action;
    }

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
}
