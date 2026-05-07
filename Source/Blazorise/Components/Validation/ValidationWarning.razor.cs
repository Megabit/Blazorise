#region Using directives
using System;
using System.Linq;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Placeholder for the <see cref="Validation"/> warning message.
/// </summary>
public partial class ValidationWarning : BaseValidationResult, IDisposable
{
    #region Members

    private bool tooltip;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        WarningMessages = ParentValidation?.Status == ValidationStatus.Warning && ParentValidation?.Messages?.Where( x => !string.IsNullOrEmpty( x ) )?.Count() > 0
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
            builder.Append( ClassProvider.ValidationWarning() );
        else
            builder.Append( ClassProvider.ValidationWarningTooltip() );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected async override void OnValidationStatusChanged( object sender, ValidationStatusChangedEventArgs eventArgs )
    {
        WarningMessages = eventArgs.Status == ValidationStatus.Warning
            ? eventArgs.Messages?.Where( x => !string.IsNullOrEmpty( x ) )?.Count() > 0
                ? eventArgs.Messages?.Where( x => !string.IsNullOrEmpty( x ) )?.ToArray()
                : null
            : null;

        await InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Custom warning messages that will override a default content.
    /// </summary>
    protected string[] WarningMessages { get; set; }

    /// <summary>
    /// If true, shows the multiline warning messages.
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