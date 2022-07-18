#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Blazorise.Utilities;
using System.Collections.Generic;
#endregion

namespace Blazorise.LoadingIndicator
{
    /// <summary>
    /// A wrapper component that adds a busy spinner or shows a loading message.
    /// Fully templatable, supports two-way binding, direct use via @ref and
    /// can be controlled by a service that may be shared by multiple instances.
    /// </summary>
    public partial class LoadingIndicator : BaseComponent, IDisposable
    {
        #region Members

        private ClassBuilder indicatorClasses;
        private StyleBuilder indicatorStyles;

        private LoadingIndicatorService service;

        private bool? loaded;
        private bool loadedParameter = true;

        private bool? busy;
        private bool busyParameter;

        #endregion

        #region Methods

        public LoadingIndicator()
        {
            indicatorClasses = new( BuildIndicatorClasses );
            indicatorStyles = new( BuildIndicatorStyles );
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "b-loading-indicator-wrapper" );
            base.BuildClasses( builder );
        }

        protected override void BuildStyles( StyleBuilder builder )
        {
            builder.Append( "position:relative", Busy && !FullScreen );
            builder.Append( "display:inline-block", Inline );
            base.BuildStyles( builder );
        }

        private void BuildIndicatorClasses( ClassBuilder builder )
        {
            builder.Append( "b-loading-indicator-overlay" );
            builder.Append( FullScreen ? "b-loading-indicator-overlay-fixed" : "b-loading-indicator-overlay-relative" );
            builder.Append( IndicatorPadding?.Class(ClassProvider), IndicatorPadding != null );
            base.BuildClasses( builder );
        }

        private void BuildIndicatorStyles( StyleBuilder builder )
        {
            builder.Append( $"background-color:{ScreenColor.Name}" );
            builder.Append( $"z-index:{ZIndex}", ZIndex.HasValue );
            builder.Append( $"justify-content:start", IndicatorHorizontalPlacement == LoadingIndicatorPlacement.Start );
            builder.Append( $"justify-content:end", IndicatorHorizontalPlacement == LoadingIndicatorPlacement.End );
            builder.Append( $"align-items:start", IndicatorVerticalPlacement == LoadingIndicatorPlacement.Top );
            builder.Append( $"align-items:end", IndicatorVerticalPlacement == LoadingIndicatorPlacement.Bottom );
            base.BuildStyles( builder );
        }

        protected override void DirtyClasses()
        {
            indicatorClasses.Dirty();
            base.DirtyClasses();
        }

        protected override void DirtyStyles()
        {
            indicatorStyles.Dirty();
            base.DirtyStyles();
        }

        /// <summary>
        /// Set component Busy state
        /// </summary>
        /// <param name="value">true or false</param>
        public void SetBusy( bool value )
        {
            if ( Busy != value )
            {
                busy = value;
                DirtyClasses();
                DirtyStyles();
                BusyChanged.InvokeAsync( value );
                InvokeAsync( StateHasChanged );
            }
        }

        /// <summary>
        /// Set component Loaded state
        /// </summary>
        /// <param name="value">true or false</param>
        public void SetLoaded( bool value )
        {
            if ( Loaded != value )
            {
                loaded = value;
                LoadedChanged.InvokeAsync( value );
                InvokeAsync( StateHasChanged );
            }
        }

        /// <inheritdoc/>
        public override Task SetParametersAsync( ParameterView parameters )
        {
            if ( parameters.TryGetValue( nameof( Loaded ), out bool newLoadedParameter ) )
            {
                if ( loadedParameter != newLoadedParameter )
                {
                    loaded = null; // use parameter instead of local value
                }
            }

            if ( parameters.TryGetValue( nameof( Busy ), out bool newBusyParameter ) )
            {
                if ( busyParameter != newBusyParameter )
                {
                    busy = null; // use parameter instead of local value
                }
            }

            DirtyClasses();
            DirtyStyles();

            return base.SetParametersAsync( parameters );
        }

        protected override void Dispose( bool disposing )
        {
            if (disposing)
            {
                Service = null;
            }

            base.Dispose( disposing );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Workaround for issue https://github.com/dotnet/aspnetcore/issues/15311
        /// Settind svg width or height to null if it had a value before throws an exception
        /// https://icons8.com/preloaders/en/search/spinner#
        /// </summary>
        private string SpinnerSVG =>
            @$"<svg viewBox='0 0 128 128' 
                {( !string.IsNullOrEmpty( SpinnerWidth ) ? $"width='{SpinnerWidth}'" : "" )}
                {( !string.IsNullOrEmpty( SpinnerHeight ) ? $"height='{SpinnerHeight}'" : "" )}>
                  <g>
                      <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerColor.Name}' />
                      <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackground.Name}' transform='rotate(45 64 64)' />
                      <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackground.Name}' transform='rotate(90 64 64)' />
                      <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackground.Name}' transform='rotate(135 64 64)' />
                      <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackground.Name}' transform='rotate(180 64 64)' />
                      <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackground.Name}' transform='rotate(225 64 64)' />
                      <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackground.Name}' transform='rotate(270 64 64)' />
                      <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackground.Name}' transform='rotate(315 64 64)' />
                      <animateTransform attributeName = 'transform' type='rotate' values='0 64 64;45 64 64;90 64 64;135 64 64;180 64 64;225 64 64;270 64 64;315 64 64' calcMode='discrete' dur='720ms' repeatCount='indefinite' />
                  </g>
              </svg>";


        /// <summary>
        /// Service used to control this instance
        /// </summary>
        [Parameter]
        public LoadingIndicatorService Service
        {
            get => service;
            set
            {
                if ( value != service )
                {
                    if ( service != null )
                    {
                        service.Unsubscribe( this );
                    }

                    service = value;
                    if ( service != null )
                    {
                        service.Subscribe( this );
                    }
                }
            }
        }

        /// <summary>
        /// Indicates whether component is ready to be rendered
        /// </summary>
        [Parameter]
        public bool Loaded
        {
            get => loaded ?? loadedParameter;
            set => loadedParameter = value;
        }

        /// <summary>
        /// Indicates whether the component should be covered with a busy screen
        /// </summary>
        [Parameter]
        public bool Busy
        {
            get => busy ?? busyParameter;
            set => busyParameter = value;
        }

        /// <summary>
        /// Occurs when IsLoaded state has changed
        /// </summary>
        [Parameter]
        public EventCallback<bool> LoadedChanged { get; set; }

        /// <summary>
        /// Occurs when IsBusy state has changed
        /// </summary>
        [Parameter]
        public EventCallback<bool> BusyChanged { get; set; }

        /// <inheritdoc/>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Busy indicator template
        /// </summary>
        [Parameter]
        public RenderFragment IndicatorTemplate { get; set; }

        /// <summary>
        /// Loading state template
        /// </summary>
        [Parameter]
        public RenderFragment LoadingTemplate { get; set; }

        /// <summary>
        /// Spinner background color
        /// </summary>
        [Parameter]
        public Background SpinnerBackground { get; set; } = "#c0c0c0";

        /// <summary>
        /// Spinner color
        /// </summary>
        [Parameter]
        public Color SpinnerColor { get; set; } = "#000000";

        /// <summary>
        /// Spinner HTML width
        /// </summary>
        [Parameter]
        public string SpinnerWidth { get; set; }

        /// <summary>
        /// Spinner HTML height
        /// </summary>
        [Parameter]
        public string SpinnerHeight { get; set; } = "64px";

        /// <summary>
        /// Indicator vertical position
        /// </summary>
        [Parameter]
        public LoadingIndicatorPlacement IndicatorVerticalPlacement { get; set; } = LoadingIndicatorPlacement.Middle;

        /// <summary>
        /// Indicator horizontal position
        /// </summary>
        [Parameter]
        public LoadingIndicatorPlacement IndicatorHorizontalPlacement { get; set; } = LoadingIndicatorPlacement.Middle;

        /// <summary>
        /// Indicator div padding
        /// </summary>
        [Parameter]
        public IFluentSpacing IndicatorPadding { get; set; }

        /// <summary>
        /// Busy screen color
        /// </summary>
        [Parameter]
        public Color ScreenColor { get; set; } = "rgba(255, 255, 255, 0.7)";

        /// <summary>
        /// Show busy indicator full screen
        /// </summary>
        [Parameter]
        public bool FullScreen { get; set; }

        /// <summary>
        /// Wrap inline content
        /// </summary>
        [Parameter]
        public bool Inline { get; set; }

        /// <summary>
        /// Overlay screen z-index
        /// </summary>
        [Parameter]
        public int? ZIndex { get; set; }

        #endregion
    }
}
