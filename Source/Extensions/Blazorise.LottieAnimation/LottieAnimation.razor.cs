#region Using directives

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

#endregion

namespace Blazorise.LottieAnimation;

/// <summary>
/// The LottieAnimation component embeds JSON-based Lottie animations into the document.
/// </summary>
public partial class LottieAnimation : BaseComponent, IAsyncDisposable
{
    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            var pathChanged     = parameters.TryGetValue<string>( nameof(Path), out var path ) && path != Path;
            var loopChanged     = parameters.TryGetValue<LoopingConfiguration>( nameof(Loop), out var loop ) && loop != Loop;
            var autoplayChanged = parameters.TryGetValue<bool>( nameof(Autoplay), out var autoPlay ) && autoPlay != Autoplay;
            var rendererChanged = parameters.TryGetValue<Renderer>( nameof(Renderer), out var renderer ) && renderer != Renderer;

            var directionChanged = parameters.TryGetValue<AnimationDirection>( nameof(Direction), out var direction ) && direction != Direction;
            var speedChanged     = parameters.TryGetValue<double>( nameof(Speed), out var speed ) && speed != Speed;

            var enteredFrameChanged = parameters.TryGetValue<EventCallback<EnteredFrameEventArgs>>( nameof(EnteredFrame), out var enteredFrame ) && enteredFrame.HasDelegate != EnteredFrame.HasDelegate;

            var reinitializationRequired = pathChanged || loopChanged || rendererChanged;
            if ( reinitializationRequired )
            {
                // Changing these settings requires a full reinitialization of the animation
                ExecuteAfterRender( async () =>
                {
                    await InitializeAnimation( new
                    {
                        path,
                        loop,
                        autoPlay,
                        renderer,
                        direction,
                        speed,
                        RegisteredEvents = GetRegisteredEvents()
                    } );
                } );
            }
            else
            {
                // These settings can be changed without reinitializing
                if ( speedChanged )
                {
                    ExecuteAfterRender( async () =>
                    {
                        await SetSpeed( speed );
                    } );
                }

                if ( directionChanged )
                {
                    ExecuteAfterRender( async () =>
                    {
                        await SetDirection( direction );
                    } );
                }

                if ( enteredFrameChanged )
                {
                    ExecuteAfterRender(async () =>
                    {
                        await UpdateRegisteredEvents();
                    } );
                }
            }
        }

        await base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        if ( JSModule == null )
        {
            DotNetObjectRef ??= DotNetObjectReference.Create( this );

            JSModule = new JSLottieAnimationModule( JSRuntime, VersionProvider );
        }

        return base.OnInitializedAsync();
    }

    /// <inheritdoc />
    protected override async Task OnFirstAfterRenderAsync()
    {
        await InitializeAnimation( new
        {
            Path,
            Loop,
            Autoplay,
            Renderer,
            Direction,
            Speed,
            RegisteredEvents = GetRegisteredEvents()
        } );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDisposeAsync();

            if ( DotNetObjectRef != null )
            {
                DotNetObjectRef.Dispose();
                DotNetObjectRef = null;
            }

            await DisposeAnimation();
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Disposes the Lottie animation and the reference
    /// </summary>
    protected async ValueTask DisposeAnimation()
    {
        if ( JSAnimationReference != null )
        {
            await JSAnimationReference.InvokeVoidAsync( "destroy" );
            await JSAnimationReference.DisposeAsync();
            JSAnimationReference = null;
        }
    }

    /// <summary>
    /// Initializes the JS animation 
    /// </summary>
    /// <param name="configuration">Animation configuration</param>
    protected virtual async ValueTask InitializeAnimation( object configuration )
    {
        await DisposeAnimation();
        JSAnimationReference = await JSModule.InitializeAnimation( DotNetObjectRef, ElementRef, ElementId, configuration );
    }

    /// <summary>
    /// Set the animation direction
    /// </summary>
    /// <param name="direction">Animation playback direction</param>
    protected virtual async ValueTask SetDirection( AnimationDirection direction )
    {
        await JSAnimationReference.InvokeVoidAsync( "setDirection", direction );
    }

    /// <summary>
    /// Set the animation playback speed
    /// </summary>
    /// <param name="speed">Animation playback speed</param>
    protected virtual async ValueTask SetSpeed( double speed )
    {
        await JSAnimationReference.InvokeVoidAsync( "setSpeed", speed );
    }

    /// <summary>
    /// Updates the animations list of registered events
    /// </summary>
    protected virtual async ValueTask UpdateRegisteredEvents()
    {
        var registeredEvents = GetRegisteredEvents();
        await JSAnimationReference.InvokeVoidAsync( "updateRegisteredEvents", registeredEvents );
    }

    /// <summary>
    /// Returns all the events that currently have registered delegates
    /// </summary>
    /// <returns>All the events that currently have registered delegates</returns>
    protected virtual HashSet<string> GetRegisteredEvents()
    {
        HashSet<string> registeredEvents = new();

        if ( EnteredFrame.HasDelegate )
        {
            registeredEvents.Add( "enterFrame" );
        }

        return registeredEvents;
    }

    #region Event Notifiers
    
    /// <summary>
    /// Notifies the lottie animation component that a new frame has been entered. Should not be called directly by the user!
    /// </summary>
    /// <param name="eventArgs">Enter frame event args</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public async ValueTask NotifyEnteredFrame(EnteredFrameEventArgs eventArgs)
    {
        await EnteredFrame.InvokeAsync( eventArgs );
    }

    #endregion

    #endregion

    #region Parameters

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Reference to the object that should be accessed through JSInterop.
    /// </summary>
    protected DotNetObjectReference<LottieAnimation> DotNetObjectRef { get; private set; }

    /// <summary>
    /// Gets or sets the <see cref="JSLottieAnimationModule"/> instance.
    /// </summary>
    protected JSLottieAnimationModule JSModule { get; private set; }

    /// <summary>
    /// Gets or sets the reference to the JS lottie animation object
    /// </summary>
    protected IJSObjectReference JSAnimationReference { get; private set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    [Inject]
    private IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Relative path to the animation object
    /// </summary>
    [Parameter]
    public string Path { get; set; }

    /// <summary>
    /// Whether or not the animation should loop, or a number of times the animation should loop.
    /// </summary>
    [Parameter]
    public LoopingConfiguration Loop { get; set; } = true;

    /// <summary>
    /// Whether or not the animation should start playing as soon as it's ready
    /// </summary>
    [Parameter]
    public bool Autoplay { get; set; } = true;

    /// <summary>
    /// Renderer to use
    /// </summary>
    [Parameter]
    public Renderer Renderer { get; set; } = Renderer.SVG;

    /// <summary>
    /// Animation playback direction
    /// </summary>
    [Parameter]
    public AnimationDirection Direction { get; set; } = AnimationDirection.Forward;

    /// <summary>
    /// Animation playback speed
    /// </summary>
    [Parameter]
    public double Speed { get; set; } = 1.0;

    /// <summary>
    /// Triggered when animation playback enters a new frame
    /// </summary>
    [Parameter]
    public EventCallback<EnteredFrameEventArgs> EnteredFrame { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="LottieAnimation"/>.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion
}