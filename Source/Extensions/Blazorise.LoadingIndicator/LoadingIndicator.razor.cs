#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Blazorise.Utilities;
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
            builder.Append( "position:relative", GetBusy() && !FullScreen );
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
            builder.Append( $"justify-content:start", SpinnerHorizontalPlacement == Placement.Start );
            builder.Append( $"justify-content:end", SpinnerHorizontalPlacement == Placement.End );
            builder.Append( $"align-items:top", SpinnerVerticalPlacement == Placement.Top );
            builder.Append( $"align-items:bottom", SpinnerVerticalPlacement == Placement.Bottom );
            base.BuildStyles( builder );
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
            if ( GetBusy() != val )
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
            if ( GetLoaded() != val )
            {
                loaded = val;
                LoadedChanged.InvokeAsync( val );
                InvokeAsync( StateHasChanged );
            }
        }

        private void Service_BusyChanged( bool val ) => SetBusy( val );
        private void Service_LoadedChanged( bool val ) => SetLoaded( val );

        private bool GetBusy() => busy ?? Busy;
        private bool GetLoaded() => loaded ?? Loaded;

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
                        service.BusyChanged -= Service_BusyChanged;
                        service.LoadedChanged -= Service_LoadedChanged;
                    }

                    service = value;
                    if ( service != null )
                    {
                        service.BusyChanged += Service_BusyChanged;
                        service.LoadedChanged += Service_LoadedChanged;
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
        /// Spinner css width
        /// </summary>
        [Parameter]
        public string SpinnerWidth { get; set; } = "64px";

        /// <summary>
        /// Spinner css height
        /// </summary>
        [Parameter]
        public string SpinnerHeight { get; set; } = "64px";

        /// <summary>
        /// Indicator vertical position
        /// </summary>
        [Parameter]
        public Placement? SpinnerVerticalPlacement { get; set; }

        /// <summary>
        /// Indicator horizontal position
        /// </summary>
        [Parameter]
        public Placement? SpinnerHorizontalPlacement { get; set; }

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
