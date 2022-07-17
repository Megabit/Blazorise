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

        private ClassBuilder screenClasses;
        private ClassBuilder indicatorClasses;
        private StyleBuilder screenStyles;
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
            screenClasses = new( BuildScreenClasses );
            screenStyles = new( BuildScreenStyles );
            indicatorClasses = new( BuildIndicatorClasses );
            indicatorStyles = new( BuildIndicatorStyles );
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "bl-wrapper" );
            base.BuildClasses( builder );
        }

        protected override void BuildStyles( StyleBuilder builder )
        {
            builder.Append( "position:relative", Busy && !FullScreen );
            builder.Append( "display:inline-block", Inline );
            base.BuildStyles( builder );
        }
        private void BuildScreenClasses( ClassBuilder builder )
        {
            builder.Append( "bl-overlay" );
            builder.Append( FullScreen ? "bl-overlay-fixed" : "bl-overlay-relative" );
            base.BuildClasses( builder );
        }
        private void BuildIndicatorClasses( ClassBuilder builder )
        {
            builder.Append( "bl-overlay" );
            builder.Append( FullScreen ? "bl-overlay-fixed" : "bl-overlay-relative" );
            builder.Append( "bl-indicator" );
            builder.Append( IndicatorPadding?.Class(ClassProvider), IndicatorPadding != null );
            base.BuildClasses( builder );
        }

        private void BuildScreenStyles( StyleBuilder builder )
        {
            builder.Append( $"opacity:{ScreenOpacity}" );
            builder.Append( $"background-color:{ScreenColor}" );
            builder.Append( $"z-index:{ZIndex}", ZIndex.HasValue );
            base.BuildStyles( builder );
        }

        private void BuildIndicatorStyles( StyleBuilder builder )
        {
            builder.Append( $"z-index:{ZIndex}", ZIndex.HasValue );
            builder.Append( $"justify-content:start", IndicatorHorizontalPlacement == Placement.Start );
            builder.Append( $"justify-content:end", IndicatorHorizontalPlacement == Placement.End );
            builder.Append( $"align-items:start", IndicatorVerticalPlacement == Placement.Top );
            builder.Append( $"align-items:end", IndicatorVerticalPlacement == Placement.Bottom );
            base.BuildStyles( builder );
        }

        private Dictionary<string, object> SpinnerAttributes
        {
            get
            {
                var attributes = new Dictionary<string, object>();
                if ( !string.IsNullOrEmpty( SpinnerWidth ) )
                {
                    attributes.Add( "width", SpinnerWidth );
                }
                if ( !string.IsNullOrEmpty( SpinnerHeight ) )
                {
                    attributes.Add( "height", SpinnerHeight );
                }
                return attributes;
            }
        }

        private void DirtyClassesAndStyles()
        {
            DirtyClasses();
            DirtyStyles();

            screenClasses.Dirty();
            screenStyles.Dirty();

            indicatorClasses.Dirty();
            indicatorStyles.Dirty();
        }

        /// <summary>
        /// Set component Busy state
        /// </summary>
        /// <param name="val">true or false</param>
        public void SetBusy( bool val )
        {
            if ( Busy != val )
            {
                busy = val;
                DirtyClassesAndStyles();
                BusyChanged.InvokeAsync( val );
                InvokeAsync( StateHasChanged );
            }
        }

        /// <summary>
        /// Set component Loaded state
        /// </summary>
        /// <param name="val">true or false</param>
        public void SetLoaded( bool val )
        {
            if ( Loaded != val )
            {
                loaded = val;
                LoadedChanged.InvokeAsync( val );
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

            return base.SetParametersAsync( parameters );
        }

        void IDisposable.Dispose()
        {
            Service = null;
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
                      <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerColor}' />
                      <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackgroundColor}' transform='rotate(45 64 64)' />
                      <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackgroundColor}' transform='rotate(90 64 64)' />
                      <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackgroundColor}' transform='rotate(135 64 64)' />
                      <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackgroundColor}' transform='rotate(180 64 64)' />
                      <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackgroundColor}' transform='rotate(225 64 64)' />
                      <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackgroundColor}' transform='rotate(270 64 64)' />
                      <path d = 'M38.52 33.37L21.36 16.2A63.6 63.6 0 0 1 59.5.16v24.3a39.5 39.5 0 0 0-20.98 8.92z' fill='{SpinnerBackgroundColor}' transform='rotate(315 64 64)' />
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
            set
            {
                busyParameter = value;
                DirtyClassesAndStyles();
            }
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
        public string SpinnerBackgroundColor { get; set; } = "#c0c0c0";

        /// <summary>
        /// Spinner color
        /// </summary>
        [Parameter]
        public string SpinnerColor { get; set; } = "#000000";

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
        public Placement IndicatorVerticalPlacement { get; set; } = Placement.Middle;

        /// <summary>
        /// Indicator horizontal position
        /// </summary>
        [Parameter]
        public Placement IndicatorHorizontalPlacement { get; set; } = Placement.Middle;

        /// <summary>
        /// Indicator div padding
        /// </summary>
        [Parameter]
        public IFluentSpacing IndicatorPadding { get; set; }

        /// <summary>
        /// Busy screen opacity
        /// </summary>
        [Parameter]
        public double ScreenOpacity { get; set; } = 0.5;

        /// <summary>
        /// Busy screen color
        /// </summary>
        [Parameter]
        public string ScreenColor { get; set; } = "white";

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
