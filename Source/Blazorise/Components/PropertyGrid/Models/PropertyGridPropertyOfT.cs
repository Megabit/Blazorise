#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Base definition for a strongly typed property.
/// </summary>
public abstract class PropertyGridProperty<TValue> : PropertyGridProperty
{
    #region Constructors

    /// <summary>
    /// Initializes a strongly typed property definition.
    /// </summary>
    protected PropertyGridProperty( string key, string label, TValue value )
        : base( key, label )
    {
        TypedValue = value;
    }

    #endregion

    #region Methods

    internal override object CreateValueChangedCallback( PropertyGridView owner )
        => EventCallback.Factory.Create<TValue>( owner, value => owner.ChangeValueAsync( this, value ) );

    #endregion

    #region Properties

    /// <summary>
    /// Gets the strongly typed property value.
    /// </summary>
    public TValue TypedValue { get; }

    /// <inheritdoc/>
    public override object Value => TypedValue;

    /// <inheritdoc/>
    public override Type ValueType => typeof( TValue );

    #endregion
}