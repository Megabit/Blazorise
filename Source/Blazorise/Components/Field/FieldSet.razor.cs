#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Compatibility wrapper that renders a grouped <see cref="Field"/>.
/// </summary>
public partial class FieldSet : Field
{
    /// <inheritdoc/>
    protected override bool ForceGroup => true;

    #region Members

    private bool horizontal;

    private List<BaseComponent> hookables;

    private Validation previousParentValidation;

    private ValidationStatus previousValidationStatus;

    private Legend legend;

    /// <summary>
    /// Raises when the legend reference changes.
    /// </summary>
    internal event Action LegendElementChanged;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        base.Dispose( disposing );
    }

    /// <summary>
    /// Unsubscribe from <see cref="Validation.StatusChanged"/> event.
    /// </summary>
    private void DetachValidationStatusChangedListener()
    {
        if ( previousParentValidation is not null )
        {
            previousParentValidation.ValidationStatusChanged -= OnValidationStatusChanged;
        }
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        base.BuildClasses( builder );
    }

    /// <summary>
    /// Handles the <see cref="Validation.StatusChanged"/> event.
    /// </summary>
    /// <param name="sender">Object that raised the event.</param>
    /// <param name="eventArgs">Data about the <see cref="Validation"/> status change event.</param>
    protected void OnValidationStatusChanged( object sender, ValidationStatusChangedEventArgs eventArgs )
    {
        if ( previousValidationStatus != eventArgs.Status )
        {
            previousValidationStatus = eventArgs.Status;

            DirtyClasses();

            InvokeAsync( StateHasChanged );
        }
    }

    /// <summary>
    /// Notifies the fieldset that one of its child components needs a special treatment.
    /// </summary>
    /// <param name="component">Reference to the child component.</param>
    internal void Hook( BaseComponent component )
    {
        hookables ??= new();

        hookables.Add( component );
    }

    /// <summary>
    /// Removes a previously registered child component hook.
    /// </summary>
    /// <param name="component">Reference to the child component.</param>
    internal void UnHook( BaseComponent component )
    {
        hookables?.Remove( component );
    }

    /// <summary>
    /// Registers the legend component inside this fieldset.
    /// </summary>
    /// <param name="legendComponent">Legend component.</param>
    internal void NotifyLegendInitialized( Legend legendComponent )
    {
        if ( legendComponent is null )
            return;

        if ( ReferenceEquals( legend, legendComponent ) )
            return;

        legend = legendComponent;
        LegendElementChanged?.Invoke();
    }

    /// <summary>
    /// Removes the legend component inside this fieldset.
    /// </summary>
    /// <param name="legendComponent">Legend component.</param>
    internal void NotifyLegendRemoved( Legend legendComponent )
    {
        if ( !ReferenceEquals( legend, legendComponent ) )
            return;

        legend = null;
        LegendElementChanged?.Invoke();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the element id of the fieldset legend.
    /// </summary>
    internal string LegendElementId => legend?.ElementId;

    /// <summary>
    /// Determines whether the form controls should be aligned horizontally, as in a horizontal form layout.
    /// </summary>
    [Parameter]
    public new bool Horizontal
    {
        get => base.Horizontal;
        set => base.Horizontal = value;
    }

    /// <summary>
    /// A reference to the parent <see cref="Validation"/> component in which this component is nested.
    /// </summary>
    [CascadingParameter] protected new Validation ParentValidation { get; set; }

    #endregion
}