#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Wrapper for form input components like label, text, button, etc.
/// </summary>
public partial class Field : BaseColumnComponent, IDisposable
{
    #region Members

    private bool horizontal;

    private JustifyContent justifyContent = JustifyContent.Default;

    private List<BaseComponent> hookables;

    private Validation previousParentValidation;

    private ValidationStatus previousValidationStatus;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        if ( ParentValidation != previousParentValidation )
        {
            DetachValidationStatusChangedListener();
            ParentValidation.ValidationStatusChanged += OnValidationStatusChanged;
            previousParentValidation = ParentValidation;
        }
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        previousValidationStatus = ParentValidation?.Status ?? ValidationStatus.None;

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            DetachValidationStatusChangedListener();

            if ( ParentValidation is not null )
            {
                ParentValidation.ValidationStatusChanged -= OnValidationStatusChanged;
            }
        }

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
        builder.Append( ClassProvider.Field() );
        builder.Append( ClassProvider.FieldHorizontal( Horizontal ) );
        builder.Append( ClassProvider.FieldJustifyContent( JustifyContent ) );
        builder.Append( ClassProvider.FieldValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

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
    /// Notifies the field that one of it's child components needs a special treatment.
    /// </summary>
    /// <param name="component">Reference to the child component.</param>
    internal void Hook( BaseComponent component )
    {
        hookables ??= new();

        hookables.Add( component );
    }

    internal void UnHook( BaseComponent component )
    {
        hookables?.Remove( component );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Determines if the field is inside of <see cref="Fields"/> component.
    /// </summary>
    protected bool IsFields => ParentFields is not null;

    /// <summary>
    /// Determines whether the form controls should be aligned horizontally, as in a horizontal form layout.
    /// </summary>
    [Parameter]
    public bool Horizontal
    {
        get => horizontal;
        set
        {
            horizontal = value;

            hookables?.ForEach( x => x.DirtyClasses() );

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines how the container's items are aligned along the main axis when there is extra space available.
    /// </summary>
    [Parameter]
    public JustifyContent JustifyContent
    {
        get => justifyContent;
        set
        {
            justifyContent = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// A reference to the parent <see cref="Fields"/> component in which this component is nested.
    /// </summary>
    [CascadingParameter] protected Fields ParentFields { get; set; }

    /// <summary>
    /// A reference to the parent <see cref="Validation"/> component in which this component is nested.
    /// </summary>
    [CascadingParameter] protected Validation ParentValidation { get; set; }

    #endregion
}