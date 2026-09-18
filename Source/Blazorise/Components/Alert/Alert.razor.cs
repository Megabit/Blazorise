#region Using directives
using System;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Provide contextual feedback messages for typical user actions with the handful of available and flexible alert messages.
/// </summary>
public partial class Alert : BaseComponent, IDisposable
{
    #region Members

    /// <summary>
    /// Holds the state of the <see cref="Alert"/> component.
    /// </summary>
    private AlertState state = new()
    {
        Color = Color.Default,
    };

    /// <summary>
    /// Flag that indicates if <see cref="Alert"/> contains the <see cref="AlertMessage"/> component.
    /// </summary>
    private bool hasMessage;

    /// <summary>
    /// Flag that indicates if <see cref="Alert"/> contains the <see cref="AlertDescription"/> component.
    /// </summary>
    private bool hasDescription;

    /// <summary>
    /// Tracks whether dismissible styling was explicitly configured.
    /// </summary>
    private ComponentParameterInfo<bool> paramDismisable;

    /// <summary>
    /// Flag that indicates if the alert contains a close button.
    /// </summary>
    private bool hasCloseButton;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Dismisable, out paramDismisable );

        if ( paramDismisable.Changed )
            DirtyClasses();

        if ( parameters.IsParameterChanged( Animated ) || parameters.IsParameterChanged( AnimationDuration ) )
        {
            DirtyClasses();
            DirtyStyles();
        }

        await base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        HandleVisibilityState( Visible );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Alert() );
        builder.Append( ClassProvider.AlertColor( Color ) );
        builder.Append( ClassProvider.AlertDismisable( EffectiveDismisable ) );
        builder.Append( ClassProvider.AlertFade( Animated ) );
        builder.Append( ClassProvider.AlertShow( true, Visible ) );
        builder.Append( ClassProvider.AlertHasMessage( hasMessage ) );
        builder.Append( ClassProvider.AlertHasDescription( hasDescription ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( StyleProvider.AlertAnimationDuration( EffectiveAnimationDuration ) );

        base.BuildStyles( builder );
    }

    /// <summary>
    /// Displays the alert to the user.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Show()
    {
        if ( Visible )
            return Task.CompletedTask;

        Visible = true;

        return InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Conceals the alert from the user.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Hide()
    {
        if ( !Visible )
            return Task.CompletedTask;

        Visible = false;

        return InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Toggles the visibility of the alert.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Toggle()
    {
        Visible = !Visible;

        return InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Sets the visibility state of this <see cref="Alert"/> component.
    /// </summary>
    /// <param name="visible">True if <see cref="Alert"/> is visible.</param>
    private void HandleVisibilityState( bool visible )
    {
        Display = visible
            ? Blazorise.Display.Always
            : Blazorise.Display.None;

        DirtyClasses();
    }

    /// <summary>
    /// Raises all registered events for this <see cref="Alert"/> component.
    /// </summary>
    /// <param name="visible">True if <see cref="Alert"/> is visible.</param>
    private void RaiseEvents( bool visible )
    {
        InvokeAsync( () => VisibleChanged.InvokeAsync( visible ) );
    }

    /// <summary>
    /// Notifies the alert that one of the child components is a message.
    /// </summary>
    internal void NotifyHasMessage()
    {
        hasMessage = true;

        DirtyClasses();
        InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Notifies the alert that one of the child components is a description.
    /// </summary>
    internal void NotifyHasDescription()
    {
        hasDescription = true;

        DirtyClasses();
        InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Registers a close button and applies automatic dismissible styling.
    /// </summary>
    internal void NotifyCloseButtonInitialized()
    {
        hasCloseButton = true;

        if ( !paramDismisable.Defined )
        {
            DirtyClasses();
            InvokeAsync( StateHasChanged );
        }
    }

    /// <summary>
    /// Unregisters the close button and removes automatic dismissible styling.
    /// </summary>
    internal void NotifyCloseButtonRemoved()
    {
        if ( Disposed || AsyncDisposed )
            return;

        hasCloseButton = false;

        if ( !paramDismisable.Defined )
        {
            DirtyClasses();
            InvokeAsync( StateHasChanged );
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets dismissible styling from the explicit parameter or close button presence.
    /// </summary>
    protected bool EffectiveDismisable => paramDismisable.Defined ? Dismisable : hasCloseButton;

    /// <summary>
    /// Gets whether the hidden alert should prevent interaction.
    /// </summary>
    protected bool IsInert => !Visible;

    /// <summary>
    /// Gets the duration override, or null to retain the provider's default timing.
    /// </summary>
    protected int? EffectiveAnimationDuration => !Animated ? 0 : AnimationDuration.HasValue ? Math.Max( 0, AnimationDuration.Value ) : null;

    /// <summary>
    /// Gets the animation duration serialized for markup.
    /// </summary>
    protected string AnimationDurationString => EffectiveAnimationDuration?.ToString( CultureInfo.InvariantCulture );

    /// <summary>
    /// Gets the readiness for subsequent entrance animations serialized for markup.
    /// </summary>
    protected string AnimationReadyString => Rendered ? "true" : null;

    /// <summary>
    /// Gets the requested visibility serialized for markup.
    /// </summary>
    protected string VisibleString => Visible ? "true" : "false";

    /// <summary>
    /// Gets the accessibility visibility state serialized for markup.
    /// </summary>
    protected string AriaHidden => !Visible ? "true" : null;

    /// <summary>
    /// Gets the reference to state object for this alert.
    /// </summary>
    protected internal AlertState State => state;

    /// <summary>
    /// Enables the transitions supplied by the CSS provider. Set to false for immediate changes.
    /// </summary>
    [Parameter] public bool Animated { get; set; } = true;

    /// <summary>
    /// Overrides the provider's animation duration, in milliseconds. Null preserves the provider's default.
    /// Zero or a negative value disables transitions.
    /// </summary>
    [Parameter] public int? AnimationDuration { get; set; }

    /// <summary>
    /// Controls the padding and positioning for close buttons.
    /// When omitted, dismissible styling is enabled automatically when a <see cref="CloseButton"/> is present.
    /// An explicit value overrides this automatic behavior.
    /// </summary>
    [Parameter]
    public bool Dismisable
    {
        get => state.Dismisable;
        set
        {
            state = state with { Dismisable = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Sets the alert visibility.
    /// </summary>
    [Parameter]
    public bool Visible
    {
        get => state.Visible;
        set
        {
            if ( value == state.Visible )
                return;

            state = state with { Visible = value };

            HandleVisibilityState( value );
            RaiseEvents( value );
        }
    }

    /// <summary>
    /// Notifies when the alert visibility state changes.
    /// This notification does not wait for the entrance or exit animation to finish.
    /// </summary>
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>
    /// Specifies the alert color.
    /// </summary>
    [Parameter]
    public Color Color
    {
        get => state.Color;
        set
        {
            state = state with { Color = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the alert intent.
    /// </summary>
    [Parameter]
    public Intent Intent
    {
        get => Color.ToIntent();
        set => Color = value.ToColor();
    }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Alert"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}