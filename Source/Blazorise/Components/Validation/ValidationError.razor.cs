#region Using directives
using System;
using System.Linq;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Placeholder for the <see cref="Validation"/> error message.
/// </summary>
public partial class ValidationError : BaseValidationResult, IDisposable
{
    #region Members

    private bool tooltip;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ErrorMessages = ParentValidation?.Messages?.Where( x => !string.IsNullOrEmpty( x ) )?.Count() > 0
            ? ParentValidation?.Messages?.Where( x => !string.IsNullOrEmpty( x ) )?.ToArray()
            : null;

        base.OnInitialized();

        ParentValidation?.NotifyValidationMessageInitialized( this );
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentValidation?.NotifyValidationMessageRemoved( this );
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( !Tooltip )
            builder.Append( ClassProvider.ValidationError() );
        else
            builder.Append( ClassProvider.ValidationErrorTooltip() );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected async override void OnValidationStatusChanged( object sender, ValidationStatusChangedEventArgs eventArgs )
    {
        if ( eventArgs.Status == ValidationStatus.Error )
        {
            ErrorMessages = eventArgs.Messages?.Where( x => !string.IsNullOrEmpty( x ) )?.Count() > 0
                ? eventArgs.Messages?.Where( x => !string.IsNullOrEmpty( x ) )?.ToArray()
                : null;
        }

        await InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Custom error messages that will override a default content.
    /// </summary>
    protected string[] ErrorMessages { get; set; }

    /// <summary>
    /// If true, shows the multiline error messages.
    /// </summary>
    [Parameter] public bool Multiline { get; set; }

    /// <summary>
    /// If true, shows the tooltip instead of label.
    /// </summary>
    [Parameter]
    public bool Tooltip
    {
        get => tooltip;
        set
        {
            tooltip = value;

            DirtyClasses();
        }
    }

    #endregion
}