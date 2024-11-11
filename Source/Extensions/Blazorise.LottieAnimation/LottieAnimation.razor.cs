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
    #region Members

    /// <summary>
    /// The current frame as reported by the animation itself
    /// </summary>
    private double? lastReportedFrame;

    /// <summary>
    /// Indicates whether or not a synchronization is required for the current frame
    /// </summary>
    private bool frameSyncRequired = false;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            var pathChanged = parameters.TryGetValue<string>( nameof( Path ), out var path ) && path != Path;
            var rendererChanged = parameters.TryGetValue<LottieAnimationRenderer>( nameof( Renderer ), out var renderer ) && renderer != Renderer;
            var directionChanged = parameters.TryGetValue<LottieAnimationDirection>( nameof( Direction ), out var direction ) && direction != Direction;
            var speedChanged = parameters.TryGetValue<double>( nameof( Speed ), out var speed ) && Math.Abs( speed - Speed ) > .001;
            var loopChanged = parameters.TryGetValue<LoopConfiguration>( nameof( Loop ), out var loop ) && loop != Loop;
            var pausedChanged = parameters.TryGetValue<bool>( nameof( Paused ), out var paused ) && paused != Paused;
            var currentFrameDelegateChanged = parameters.TryGetValue<EventCallback<double>>( nameof( CurrentFrameChanged ), out var currentFrameChanged ) && ( currentFrameChanged.HasDelegate != CurrentFrameChanged.HasDelegate );

            // Frame synchronization is required whenever the user manually changes the value of the CurrentFrame
            frameSyncRequired = parameters.TryGetValue<double>( nameof( CurrentFrame ), out var currentFrame )
                                 && ( lastReportedFrame.HasValue && Math.Abs( currentFrame - lastReportedFrame.Value ) > .001 );

            // Changing the path or renderer requires us to fully reinitialize the animation
            var reinitializationRequired = pathChanged || rendererChanged;

            if ( reinitializationRequired )
            {
                ExecuteAfterRender( SynchronizeAnimation );
            }
            else
            {
                ExecuteAfterRender( async () =>
                {
                    var tasks = new List<Task>();

                    if ( currentFrameDelegateChanged )
                    {
                        tasks.Add( SynchronizeSendCurrentFrame() );
                    }

                    if ( speedChanged )
                    {
                        tasks.Add( SynchronizeSpeed() );
                    }

                    if ( directionChanged )
                    {
                        tasks.Add( SynchronizeDirection() );
                    }

                    if ( loopChanged )
                    {
                        tasks.Add( SynchronizeLoop() );
                    }

                    if ( frameSyncRequired )
                    {
                        tasks.Add( SynchronizeCurrentFrame() );
                    }
                    else if ( pausedChanged )
                    {
                        // Synchronizing the current frame will already synchronize the pause setting, so we only need
                        // to do it manually if we're not synchronizing the frame
                        tasks.Add( SynchronizePaused() );
                    }

                    await Task.WhenAll( tasks );
                } );
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
            JSModule = new JSLottieAnimationModule( JSRuntime, VersionProvider, BlazoriseOptions );
        }

        return base.OnInitializedAsync();
    }

    /// <inheritdoc />
    protected override async Task OnFirstAfterRenderAsync()
    {
        // Perform the initial load of the animation
        await SynchronizeAnimation();
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
    /// Synchronizes the JS animation with the current animation configuration
    /// </summary>
    protected virtual async Task SynchronizeAnimation()
    {
        await DisposeAnimation();
        JSAnimationReference = await JSModule.InitializeAnimation(DotNetObjectRef, ElementRef, ElementId, new LottieAnimationInitializeJSOptions
        {
            Path = Path,
            Loop = Loop,
            Autoplay = !Paused,
            Renderer = Renderer,
            Direction = Direction,
            Speed = Speed,
            SendCurrentFrame = CurrentFrameChanged.HasDelegate
        });

    }

    /// <summary>
    /// Synchronizes the JS animation direction with the current direction
    /// </summary>
    protected virtual async Task SynchronizeDirection()
    {
        await JSAnimationReference.InvokeVoidAsync( "setDirection", Direction );
    }

    /// <summary>
    /// Synchronizes the JS animation speed with the current speed
    /// </summary>
    protected virtual async Task SynchronizeSpeed()
    {
        await JSAnimationReference.InvokeVoidAsync( "setSpeed", Speed );
    }

    /// <summary>
    /// Synchronizes the JS animation loop setting with the current loop setting
    /// </summary>
    protected virtual async Task SynchronizeLoop()
    {
        await JSAnimationReference.InvokeVoidAsync( "setLoop", Loop );
    }

    /// <summary>
    /// Synchronizes the JS animation current frame with the current frame
    /// </summary>
    protected virtual async Task SynchronizeCurrentFrame()
    {
        if ( Paused )
        {
            await JSAnimationReference.InvokeVoidAsync( "goToAndStop", CurrentFrame, true );
        }
        else
        {
            await JSAnimationReference.InvokeVoidAsync( "goToAndPlay", CurrentFrame, true );
        }

        frameSyncRequired = false;
    }

    /// <summary>
    /// Synchronizes the JS animation playback with the current playback
    /// </summary>
    protected virtual async Task SynchronizePaused()
    {
        if ( Paused )
        {
            await JSAnimationReference.InvokeVoidAsync( "pause" );
        }
        else
        {
            await JSAnimationReference.InvokeVoidAsync( "play" );
        }
    }

    /// <summary>
    /// Enables or disables the sending of the Current frame notification
    ///
    /// The current frame event is triggered extremely frequently, so we only send it if someone is listening.
    /// </summary>
    protected virtual async Task SynchronizeSendCurrentFrame()
    {
        await JSAnimationReference.InvokeVoidAsync( "setSendCurrentFrame", CurrentFrameChanged.HasDelegate );
    }

    #region Event Notifiers

    /// <summary>
    /// Notifies the lottie animation component that the animation has completed. Should not be called directly by the user!
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyCompleted()
    {
        return Completed.InvokeAsync();
    }

    /// <summary>
    /// Notifies the lottie animation component that an animation loop has completed. Should not be called directly by the user!
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyLoopCompleted()
    {
        return LoopCompleted.InvokeAsync();
    }

    /// <summary>
    /// Notifies the lottie animation component that the animation has finished loading. Should not be called directly by the user!
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyLoaded( LottieAnimationLoadedEventArgs args )
    {
        return Loaded.InvokeAsync( args );
    }

    /// <summary>
    /// Notifies the lottie animation component that the current frame has changed. Should not be called directly by the user!
    /// </summary>
    /// <param name="currentFrame">Enter frame event args</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public async Task NotifyCurrentFrameChanged( double currentFrame )
    {
        if ( frameSyncRequired )
        {
            // We're in the process of manually setting the current frame on the animation, so skip this update.
            return;
        }

        lastReportedFrame = currentFrame;
        await CurrentFrameChanged.InvokeAsync( currentFrame );
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

    /// <summary>
    /// Gets or sets the JS runtime.
    /// </summary>
    [Inject] private IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// Gets or sets the version provider.
    /// </summary>
    [Inject] private IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Gets or sets the blazorise options.
    /// </summary>
    [Inject] protected BlazoriseOptions BlazoriseOptions { get; set; }

    /// <summary>
    /// Relative or absolute path to the animation object
    /// </summary>
    [EditorRequired]
    [Parameter] public string Path { get; set; }

    /// <summary>
    /// Whether or not the animation should loop, or a number of times the animation should loop.
    /// </summary>
    [Parameter] public LoopConfiguration Loop { get; set; } = true;

    /// <summary>
    /// Renderer to use
    /// </summary>
    [Parameter] public LottieAnimationRenderer Renderer { get; set; } = LottieAnimationRenderer.Svg;

    /// <summary>
    /// Animation playback direction
    /// </summary>
    [Parameter] public LottieAnimationDirection Direction { get; set; } = LottieAnimationDirection.Forward;

    /// <summary>
    /// Animation playback speed
    /// </summary>
    [Parameter] public double Speed { get; set; } = 1.0;

    /// <summary>
    /// Whether or not the animation is paused
    /// </summary>
    [Parameter] public bool Paused { get; set; } = false;

    /// <summary>
    /// Current playback frame
    /// </summary>
    [Parameter] public double CurrentFrame { get; set; } = 0;

    /// <summary>
    /// Called when the current frame changes
    ///
    /// Warning: This event is triggered extremely frequently. Subscribing to this event can cause a significant increase
    /// in the amount of messages sent over the websocket if using Blazor Server.
    /// </summary>
    [Parameter] public EventCallback<double> CurrentFrameChanged { get; set; }

    /// <summary>
    /// Called when the animation completes
    /// </summary>
    [Parameter] public EventCallback Completed { get; set; }

    /// <summary>
    /// Called when a loop completes
    /// </summary>
    [Parameter] public EventCallback LoopCompleted { get; set; }

    /// <summary>
    /// Called when the animation finishes loading and the elements have been added to the DOM
    /// </summary>
    [Parameter] public EventCallback<LottieAnimationLoadedEventArgs> Loaded { get; set; }

    #endregion
}