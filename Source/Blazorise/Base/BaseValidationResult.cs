#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Base class for validation result messages.
/// </summary>
public abstract class BaseValidationResult : BaseComponent, IDisposable
{
    #region Members

    private Validation previousParentValidation;

    #endregion

    #region Constructors

    /// <summary>
    /// A default constructors for <see cref="BaseValidationResult"/>.
    /// </summary>
    public BaseValidationResult()
    {
    }

    #endregion

    #region Methods

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

    private void DetachValidationStatusChangedListener()
    {
        if ( previousParentValidation is not null )
        {
            previousParentValidation.ValidationStatusChanged -= OnValidationStatusChanged;
        }
    }

    /// <inheritdoc/>
    protected virtual async void OnValidationStatusChanged( object sender, ValidationStatusChangedEventArgs eventArgs )
    {
        await InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the reference to the parent validation.
    /// </summary>
    [CascadingParameter] protected Validation ParentValidation { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="BaseValidationResult"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}