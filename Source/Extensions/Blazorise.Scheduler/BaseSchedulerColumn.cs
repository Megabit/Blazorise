#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Defines a scheduler column that maps an additional item field to custom editor and display templates.
/// </summary>
/// <typeparam name="TItem">The scheduler item type.</typeparam>
public abstract class BaseSchedulerColumn<TItem> : ComponentBase, IDisposable
{
    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentScheduler?.AddColumn( this );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ParentScheduler?.RemoveColumn( this );
    }

    /// <summary>
    /// Gets the column value from an item.
    /// </summary>
    /// <param name="item">The item from which to read the value.</param>
    /// <returns>The current value.</returns>
    internal abstract object GetValue( TItem item );

    /// <summary>
    /// Sets the column value on an item.
    /// </summary>
    /// <param name="item">The item on which to set the value.</param>
    /// <param name="value">The new value.</param>
    internal abstract void SetValue( TItem item, object value );

    /// <summary>
    /// Renders the column editor.
    /// </summary>
    /// <param name="item">The item being edited.</param>
    /// <param name="editState">The scheduler edit state.</param>
    /// <returns>The editor template, if defined.</returns>
    internal abstract RenderFragment RenderEdit( TItem item, SchedulerEditState editState );

    /// <summary>
    /// Renders the column editor with an externally managed value.
    /// </summary>
    /// <param name="item">The item being edited.</param>
    /// <param name="value">The current editor value.</param>
    /// <param name="valueChanged">The callback that updates the editor value.</param>
    /// <param name="editState">The scheduler edit state.</param>
    /// <returns>The editor template, if defined.</returns>
    internal abstract RenderFragment RenderEdit( TItem item, object value, Action<object> valueChanged, SchedulerEditState editState );

    /// <summary>
    /// Renders the column display template.
    /// </summary>
    /// <param name="item">The item being rendered.</param>
    /// <param name="isRecurring">Indicates whether the item is recurring.</param>
    /// <returns>The display template, if defined.</returns>
    internal abstract RenderFragment RenderDisplay( TItem item, bool isRecurring );

    #endregion

    #region Properties

    /// <summary>
    /// Gets whether the column can be edited in the scheduler item modal.
    /// </summary>
    internal bool IsEditable => Editable && EditTemplateAvailable;

    /// <summary>
    /// Gets whether the column can be rendered in the scheduler item display.
    /// </summary>
    internal bool IsDisplayable => Displayable && DisplayTemplateAvailable;

    /// <summary>
    /// Gets whether an edit template is available.
    /// </summary>
    protected abstract bool EditTemplateAvailable { get; }

    /// <summary>
    /// Gets whether a display template is available.
    /// </summary>
    protected abstract bool DisplayTemplateAvailable { get; }

    /// <summary>
    /// Gets or sets the parent scheduler.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> ParentScheduler { get; set; }

    /// <summary>
    /// Gets or sets the field name mapped by this scheduler column.
    /// </summary>
    [Parameter] public string Field { get; set; }

    /// <summary>
    /// Gets or sets the column caption shown in the scheduler item modal.
    /// </summary>
    [Parameter] public string Caption { get; set; }

    /// <summary>
    /// Gets or sets whether the column editor is shown in the scheduler item modal.
    /// </summary>
    [Parameter] public bool Editable { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the column display template is shown in the scheduler item display.
    /// </summary>
    [Parameter] public bool Displayable { get; set; } = true;

    #endregion
}