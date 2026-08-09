#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Snackbar.Utils;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Snackbar;

/// <summary>
/// Renders an interactive command within a snackbar notification.
/// </summary>
public partial class SnackbarAction : BaseComponent, IDisposable
{
    #region Members

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        ParentSnackbar?.NotifySnackbarActionInitialized( this );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentSnackbar?.NotifySnackbarActionRemoved( this );
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "snackbar-action-button" );
        builder.Append( $"snackbar-action-button-{ParentSnackbar.Color.GetName()}", ParentSnackbar != null && ParentSnackbar.Color != SnackbarColor.Default );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Forwards browser activation to the action callback.
    /// </summary>
    protected Task ClickHandler()
    {
        return Clicked.InvokeAsync();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Notifies when the button is clicked.
    /// </summary>
    [Parameter] public EventCallback Clicked { get; set; }

    /// <summary>
    /// Notification that owns and styles this action.
    /// </summary>
    [CascadingParameter] protected Snackbar ParentSnackbar { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="SnackbarAction"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}