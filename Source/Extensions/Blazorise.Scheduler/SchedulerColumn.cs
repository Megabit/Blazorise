using System;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Scheduler;

/// <summary>
/// Defines a typed scheduler column that maps an additional item field to custom editor and display templates.
/// </summary>
/// <typeparam name="TItem">The scheduler item type.</typeparam>
/// <typeparam name="TValue">The scheduler column value type.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public class SchedulerColumn<TItem, TValue> : BaseSchedulerColumn<TItem>
{
    #region Members

    private readonly Lazy<Func<TItem, TValue>> valueGetter;

    private readonly Lazy<Action<TItem, TValue>> valueSetter;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerColumn{TItem, TValue}"/> class.
    /// </summary>
    public SchedulerColumn()
    {
        valueGetter = new( () => Utilities.SchedulerFunctionCompiler.CreateValueGetter<TItem, TValue>( Field ) );
        valueSetter = new( () => Utilities.SchedulerFunctionCompiler.CreateValueSetter<TItem, TValue>( Field ) );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    internal override object GetValue( TItem item )
    {
        return valueGetter.Value.Invoke( item );
    }

    /// <inheritdoc/>
    internal override void SetValue( TItem item, object value )
    {
        valueSetter.Value.Invoke( item, value is null ? default : (TValue)value );
    }

    /// <inheritdoc/>
    internal override RenderFragment RenderEdit( TItem item, SchedulerEditState editState )
    {
        if ( EditTemplate is null )
            return null;

        var value = valueGetter.Value.Invoke( item );
        SchedulerColumnEditContext<TItem, TValue> context = null;
        var valueChanged = EventCallback.Factory.Create<TValue>( this, value => context.Value = value );

        context = new SchedulerColumnEditContext<TItem, TValue>( item, this, value, valueChanged, editState, value => valueSetter.Value.Invoke( item, value ) );

        return EditTemplate( context );
    }

    /// <inheritdoc/>
    internal override RenderFragment RenderEdit( TItem item, object value, Action<object> valueChanged, SchedulerEditState editState )
    {
        if ( EditTemplate is null )
            return null;

        SchedulerColumnEditContext<TItem, TValue> context = null;
        var valueChangedCallback = EventCallback.Factory.Create<TValue>( this, value => context.Value = value );

        context = new SchedulerColumnEditContext<TItem, TValue>( item, this, CoerceValue( value ), valueChangedCallback, editState, value => valueChanged?.Invoke( value ) );

        return EditTemplate( context );
    }

    private static TValue CoerceValue( object value )
    {
        return value is null ? default : (TValue)value;
    }

    /// <inheritdoc/>
    internal override RenderFragment RenderDisplay( TItem item, bool isRecurring )
    {
        if ( DisplayTemplate is null )
            return null;

        return DisplayTemplate( new SchedulerColumnDisplayContext<TItem, TValue>( item, this, valueGetter.Value.Invoke( item ), isRecurring ) );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool EditTemplateAvailable => EditTemplate is not null;

    /// <inheritdoc/>
    protected override bool DisplayTemplateAvailable => DisplayTemplate is not null;

    /// <summary>
    /// Gets or sets the template used to edit the column value in the scheduler item modal.
    /// </summary>
    [Parameter] public RenderFragment<SchedulerColumnEditContext<TItem, TValue>> EditTemplate { get; set; }

    /// <summary>
    /// Gets or sets the template used to display the column value in scheduler items.
    /// </summary>
    [Parameter] public RenderFragment<SchedulerColumnDisplayContext<TItem, TValue>> DisplayTemplate { get; set; }

    #endregion
}