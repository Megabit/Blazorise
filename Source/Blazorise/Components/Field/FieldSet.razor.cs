#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Represents a native <c>fieldset</c> element.
/// </summary>
public partial class FieldSet : BaseColumnComponent, IDisposable
{
    #region Members

    private bool horizontal;

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

            if ( ParentValidation is not null )
            {
                ParentValidation.ValidationStatusChanged += OnValidationStatusChanged;
            }

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
        }

        base.Dispose( disposing );
    }

    /// <summary>
    /// Unsubscribe from <see cref="Validation.ValidationStatusChanged"/> event.
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
        builder.Append( ClassProvider.FieldSet() );
        builder.Append( ClassProvider.FieldSetHorizontal( Horizontal ) );
        builder.Append( ClassProvider.FieldSetValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Handles the <see cref="Validation.ValidationStatusChanged"/> event.
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

    #endregion

    #region Properties

    /// <summary>
    /// Determines whether the enclosed controls should be aligned horizontally.
    /// </summary>
    [Parameter]
    public bool Horizontal
    {
        get => horizontal;
        set
        {
            horizontal = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the parent validation component.
    /// </summary>
    [CascadingParameter] protected Validation ParentValidation { get; set; }

    #endregion
}