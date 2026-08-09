#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Provides contextual information for rendering a scheduler column edit template.
/// </summary>
/// <typeparam name="TItem">The scheduler item type.</typeparam>
/// <typeparam name="TValue">The scheduler column value type.</typeparam>
public class SchedulerColumnEditContext<TItem, TValue>
{
    #region Members

    private TValue value;

    private readonly Action<TValue> valueSetter;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerColumnEditContext{TItem, TValue}"/> class.
    /// </summary>
    /// <param name="item">The item being edited.</param>
    /// <param name="column">The column being edited.</param>
    /// <param name="value">The current column value.</param>
    /// <param name="valueChanged">The callback that updates the column value.</param>
    /// <param name="editState">The scheduler edit state.</param>
    /// <param name="valueSetter">The action that applies the updated value.</param>
    internal SchedulerColumnEditContext( TItem item, SchedulerColumn<TItem, TValue> column, TValue value, EventCallback<TValue> valueChanged, SchedulerEditState editState, Action<TValue> valueSetter )
    {
        Item = item;
        Column = column;
        this.value = value;
        ValueChanged = valueChanged;
        EditState = editState;
        this.valueSetter = valueSetter;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the item being edited.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Gets the column being edited.
    /// </summary>
    public SchedulerColumn<TItem, TValue> Column { get; }

    /// <summary>
    /// Gets or sets the current column value.
    /// </summary>
    public TValue Value
    {
        get => value;
        set
        {
            this.value = value;
            valueSetter?.Invoke( value );
        }
    }

    /// <summary>
    /// Gets the callback that updates the column value.
    /// </summary>
    public EventCallback<TValue> ValueChanged { get; }

    /// <summary>
    /// Gets the scheduler edit state.
    /// </summary>
    public SchedulerEditState EditState { get; }

    #endregion
}