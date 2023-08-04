#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Blazorise.Snackbar.Utils;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Snackbar;

/// <summary>
/// Snackbars provide brief messages about app processes. The component is also known as a toast.
/// </summary>
public partial class Snackbar : BaseComponent, IDisposable
{
    #region Members

    // A unique key associated with this snackbar.
    private string key;

    /// <summary>
    /// Indicates if snackbar is visible.
    /// </summary>
    private bool visible;

    /// <summary>
    /// Indicates if snackbar can show multiple lines of text.
    /// </summary>
    private bool multiline;

    /// <summary>
    /// Snackbar location.
    /// </summary>
    private SnackbarLocation location;

    /// <summary>
    /// Snackbar color.
    /// </summary>
    private SnackbarColor snackbarColor = SnackbarColor.Default;

    /// <summary>
    /// Timer used to countdown the close event.
    /// </summary>
    private CountdownTimer countdownTimer;

    /// <summary>
    /// A flag that indicates if the snackbar is currently being closed with animation.
    /// </summary>
    private bool closeAnimation = false;

    /// <summary>
    /// A flag that indicates if the snackbar close action was delayed.
    /// </summary>
    private bool closingDelayed = false;

    /// <summary>
    /// Holds the last received reason for snackbar closure.
    /// </summary>
    private SnackbarCloseReason closeReason = SnackbarCloseReason.None;

    /// <summary>
    /// List of all action buttons placed inside the snackbar.
    /// </summary>
    private List<SnackbarAction> snackbarActions = new();

    #endregion

    #region Methods

    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "snackbar" );
        builder.Append( "snackbar-show", Visible && !closeAnimation );
        builder.Append( closeAnimation ? "snackbar-hide" : null );
        builder.Append( "snackbar-multi-line", Multiline );
        builder.Append( $"snackbar-{Location.GetName()}" );
        builder.Append( $"snackbar-{Color.GetName()}", Color != SnackbarColor.Default );

        base.BuildClasses( builder );
    }

    protected override void BuildStyles( StyleBuilder builder )
    {
        var baseTransitionDuration = AnimationInterval;

        builder.Append( FormattableString.Invariant( $"--transition-duration-desktop: {baseTransitionDuration:0}ms;" ) );
        builder.Append( FormattableString.Invariant( $"--transition-duration-desktop-complex: {baseTransitionDuration * 1.25:0}ms" ) );
        builder.Append( FormattableString.Invariant( $"--transition-duration-desktop-entering: {baseTransitionDuration * 0.75:0}ms" ) );
        builder.Append( FormattableString.Invariant( $"--transition-duration-desktop-leaving: {baseTransitionDuration * 0.65:0}ms" ) );

        builder.Append( FormattableString.Invariant( $"--transition-duration-mobile: {baseTransitionDuration * 1.5:0}ms" ) );
        builder.Append( FormattableString.Invariant( $"--transition-duration-mobile-complex: {baseTransitionDuration * 1.875:0}ms" ) );
        builder.Append( FormattableString.Invariant( $"--transition-duration-mobile-entering: {baseTransitionDuration * 1.125:0}ms" ) );
        builder.Append( FormattableString.Invariant( $"--transition-duration-mobile-leaving: {baseTransitionDuration * 0.975:0}ms" ) );

        builder.Append( FormattableString.Invariant( $"--transition-duration-tablet: {baseTransitionDuration * 1.95:0}ms" ) );
        builder.Append( FormattableString.Invariant( $"--transition-duration-tablet-complex: {baseTransitionDuration * 2.435:0}ms" ) );
        builder.Append( FormattableString.Invariant( $"--transition-duration-tablet-entering: {baseTransitionDuration * 1.46:0}ms" ) );
        builder.Append( FormattableString.Invariant( $"--transition-duration-tablet-leaving: {baseTransitionDuration * 1.265:0}ms" ) );

        base.BuildStyles( builder );
    }

    protected override void OnInitialized()
    {
        if ( countdownTimer == null )
        {
            countdownTimer = new CountdownTimer( Interval );
            countdownTimer.Elapsed += OnCountdownTimerElapsed;
        }

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            if ( countdownTimer != null )
            {
                countdownTimer.Elapsed -= OnCountdownTimerElapsed;
                countdownTimer.Dispose();
                countdownTimer = null;
            }
        }

        base.Dispose( disposing );
    }

    protected async Task OnClickHandler()
    {
        if ( DelayCloseOnClick && !closingDelayed )
        {
            countdownTimer?.Delay( DelayCloseOnClickInterval ?? Interval );
            closingDelayed = true;
        }
        else
        {
            await Hide( SnackbarCloseReason.UserClosed );
        }
    }

    /// <summary>
    /// Shows the snackbar.
    /// </summary>
    public Task Show()
    {
        if ( Visible )
            return Task.CompletedTask;

        Visible = true;
        closeAnimation = false;
        closingDelayed = false;

        return InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Hides the snackbar.
    /// </summary>
    public Task Hide()
    {
        return Hide( SnackbarCloseReason.UserClosed );
    }

    protected async Task Hide( SnackbarCloseReason closeReason )
    {
        closeAnimation = true;
        DirtyClasses();
        DirtyStyles();

        await InvokeAsync( StateHasChanged );

        await Task.Delay( (int)CloseAnimationInterval );

        closeAnimation = false;
        DirtyClasses();
        DirtyStyles();

        await InvokeAsync( StateHasChanged );

        if ( !Visible )
            return;

        this.closeReason = closeReason;

        Visible = false;

        this.closeReason = SnackbarCloseReason.None;

        await InvokeAsync( StateHasChanged );
    }

    private void OnCountdownTimerElapsed( object sender, EventArgs e )
    {
        InvokeAsync( () => Hide( SnackbarCloseReason.None ) );
    }

    private void HandleVisibilityStyles( bool visible )
    {
        if ( visible )
        {
            ExecuteAfterRender( () =>
            {
                countdownTimer?.Start();

                return Task.CompletedTask;
            } );
        }

        DirtyClasses();
        DirtyStyles();
    }

    private void RaiseEvents( bool visible )
    {
        if ( !visible )
        {
            InvokeAsync( () => Closed.InvokeAsync( new( Key, closeReason ) ) );
        }
    }

    internal void NotifySnackbarActionInitialized( SnackbarAction snackbarAction )
    {
        if ( snackbarAction == null )
            return;

        if ( !snackbarActions.Contains( snackbarAction ) )
            snackbarActions.Add( snackbarAction );
    }

    internal void NotifySnackbarActionRemoved( SnackbarAction snackbarAction )
    {
        if ( snackbarAction == null )
            return;

        if ( snackbarActions.Contains( snackbarAction ) )
            snackbarActions.Remove( snackbarAction );
    }

    #endregion

    #region Properties

    protected bool HasSnackbarActions => snackbarActions.Count > 0;

    /// <summary>
    /// Gets the duration of the closing animation of the snackbar.
    /// </summary>
    protected double CloseAnimationInterval => AnimationInterval;

    /// <summary>
    /// Gets or sets the cascaded parent SnackbarStack component.
    /// </summary>
    [CascadingParameter] protected SnackbarStack ParentSnackbar { get; set; }

    /// <summary>
    /// Unique key associated by this snackbar.
    /// </summary>
    [Parameter]
    public string Key
    {
        get => key ??= $"Snackbar_{IdGenerator.Generate}";
        set => key = value;
    }

    /// <summary>
    /// Defines the visibility of snackbar.
    /// </summary>
    [Parameter]
    public bool Visible
    {
        get => visible;
        set
        {
            if ( visible == value )
                return;

            visible = value;

            HandleVisibilityStyles( visible );
            RaiseEvents( visible );
        }
    }

    /// <summary>
    /// Allow snackbar to show multiple lines of text.
    /// </summary>
    [Parameter]
    public bool Multiline
    {
        get => multiline;
        set
        {
            multiline = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the snackbar location.
    /// </summary>
    [Parameter]
    public SnackbarLocation Location
    {
        get => location;
        set
        {
            location = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the snackbar color.
    /// </summary>
    [Parameter]
    public SnackbarColor Color
    {
        get => snackbarColor;
        set
        {
            snackbarColor = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the interval(in milliseconds) after which the snackbar will be automatically closed.
    /// </summary>
    [Parameter] public double Interval { get; set; } = Constants.DefaultIntervalBeforeClose;

    /// <summary>
    /// Defines the base animation interval in milliseconds.
    /// </summary>
    [Parameter] public double AnimationInterval { get; set; } = 200;

    /// <summary>
    /// If clicked on snackbar, a close action will be delayed by increasing the <see cref="Interval"/> time.
    /// </summary>
    [Parameter] public bool DelayCloseOnClick { get; set; }

    /// <summary>
    /// Defines the interval(in milliseconds) by which the snackbar will be delayed from closing.
    /// </summary>
    [Parameter] public double? DelayCloseOnClickInterval { get; set; }

    /// <summary>
    /// Occurs after the snackbar has closed.
    /// </summary>
    [Parameter] public EventCallback<SnackbarClosedEventArgs> Closed { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Snackbar"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
