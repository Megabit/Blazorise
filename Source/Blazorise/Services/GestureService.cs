#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// Default implementation of the gesture binding service.
/// </summary>
public class GestureService : IGestureService
{
    #region Members

    private readonly IJSGesturesModule jsGesturesModule;

    private readonly IIdGenerator idGenerator;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor for <see cref="GestureService"/>.
    /// </summary>
    /// <param name="jsGesturesModule">Gestures JS module.</param>
    /// <param name="idGenerator">ID generator.</param>
    public GestureService( IJSGesturesModule jsGesturesModule, IIdGenerator idGenerator )
    {
        this.jsGesturesModule = jsGesturesModule;
        this.idGenerator = idGenerator;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public ValueTask<IGestureSubscription> Attach( ElementReference elementRef, GestureOptions options, GestureEventHandlers eventHandlers )
        => Attach( elementRef, null, options, eventHandlers );

    /// <inheritdoc/>
    public async ValueTask<IGestureSubscription> Attach( ElementReference elementRef, string elementId, GestureOptions options, GestureEventHandlers eventHandlers )
    {
        GestureSubscription subscription = new( jsGesturesModule, elementRef, string.IsNullOrEmpty( elementId ) ? idGenerator.Generate : elementId, eventHandlers );

        await subscription.Initialize( options, eventHandlers );

        return subscription;
    }

    private static GesturesJSOptions ToJSOptions( GestureOptions options, GestureEventHandlers eventHandlers )
    {
        options ??= new();
        eventHandlers ??= GestureEventHandlers.Empty;

        return new()
        {
            Disabled = options.Disabled,
            Direction = options.Direction,
            SwipeThreshold = options.SwipeThreshold,
            SwipeVelocityThreshold = options.SwipeVelocityThreshold,
            TapMaximumDistance = options.TapMaximumDistance,
            TapMaximumDuration = options.TapMaximumDuration,
            LongPressDuration = options.LongPressDuration,
            LongPressMoveTolerance = options.LongPressMoveTolerance,
            MoveThrottleInterval = options.MoveThrottleInterval,
            TouchAction = options.TouchAction,
            PreventNativeDrag = options.PreventNativeDrag,
            NotifyGestureStarted = eventHandlers.GestureStarted is not null,
            NotifyGestureMoved = eventHandlers.GestureMoved is not null,
            NotifyGestureEnded = eventHandlers.GestureEnded is not null,
            NotifySwiped = eventHandlers.Swiped is not null,
            NotifyTapped = eventHandlers.Tapped is not null,
            NotifyLongPressed = eventHandlers.LongPressed is not null,
        };
    }

    #endregion

    private sealed class GestureSubscription : IGestureSubscription
    {
        #region Members

        private readonly IJSGesturesModule jsGesturesModule;

        private readonly ElementReference elementRef;

        private readonly string elementId;

        private readonly GestureAdapter adapter;

        private readonly DotNetObjectReference<GestureAdapter> dotNetObjectRef;

        private bool initialized;

        private bool disposed;

        #endregion

        #region Constructors

        public GestureSubscription( IJSGesturesModule jsGesturesModule, ElementReference elementRef, string elementId, GestureEventHandlers eventHandlers )
        {
            this.jsGesturesModule = jsGesturesModule;
            this.elementRef = elementRef;
            this.elementId = elementId;

            adapter = new( eventHandlers );
            dotNetObjectRef = DotNetObjectReference.Create( adapter );
        }

        #endregion

        #region Methods

        public async ValueTask Initialize( GestureOptions options, GestureEventHandlers eventHandlers )
        {
            await jsGesturesModule.Initialize( dotNetObjectRef, elementRef, elementId, ToJSOptions( options, eventHandlers ) );

            initialized = true;
        }

        /// <inheritdoc/>
        public async ValueTask Update( GestureOptions options, GestureEventHandlers eventHandlers )
        {
            if ( disposed )
                return;

            adapter.Update( eventHandlers );

            if ( initialized )
            {
                await jsGesturesModule.UpdateOptions( elementRef, elementId, ToJSOptions( options, eventHandlers ) );
            }
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            if ( disposed )
                return;

            disposed = true;

            if ( initialized )
            {
                initialized = false;

                await jsGesturesModule.SafeDestroy( elementRef, elementId );
            }

            dotNetObjectRef.Dispose();
        }

        #endregion
    }
}