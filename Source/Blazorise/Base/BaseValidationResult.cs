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
    protected override void OnInitialized()
    {
        if ( ParentValidation is not null )
        {
            ParentValidation.ValidationStatusChanged += OnValidationStatusChanged;
        }

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing && ParentValidation is not null )
        {
            ParentValidation.ValidationStatusChanged -= OnValidationStatusChanged;
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected virtual async void OnValidationStatusChanged( object sender, ValidationStatusChangedEventArgs eventArgs )
    {
        await InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Provides the reference to the parent validation.
    /// </summary>
    [CascadingParameter] protected Validation ParentValidation { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="BaseValidationResult"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}