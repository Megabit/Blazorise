#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Blazorise.Utilities;
#endregion

namespace Blazorise.BusyLoading
{
    /// <summary>
    /// A wrapper component that adds a busy spinner or show a loading message.
    /// Fully templatable, supports two-way binding, direct use using @ref and
    /// can be controlled by a service that may be shared by multiple instances.
    /// </summary>
    public partial class BusyLoading : BaseComponent, IDisposable
    {
        #region Members
        
        private BusyLoadingService service;

        private bool? isLoaded;
        private bool isLoadedParameter;
        
        private bool? isBusy;
        private bool isBusyParameter;

        #endregion

        #region Methods
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "bl-wrapper" );
            builder.Append( "bl-wrapper-container", GetIsBusy() && !IsFullScreen );
            base.BuildClasses( builder );
        }

        /// <summary>
        /// Set component IsBusy
        /// </summary>
        /// <param name="val">true or false</param>
        public void Busy( bool val )
        {
            if ( GetIsBusy() != val )
            {
                isBusy = val;
                DirtyClasses();
                IsBusyChanged.InvokeAsync( val );
                InvokeAsync( StateHasChanged );
            }
        }

        /// <summary>
        /// Set component IsLoaded
        /// </summary>
        /// <param name="val">true or false</param>
        public void Loaded( bool val )
        {
            if ( GetIsLoaded() != val )
            {
                isLoaded = val;
                IsLoadedChanged.InvokeAsync( val );
                InvokeAsync( StateHasChanged );
            }
        }

        private void Service_BusyChanged( bool val ) => Busy( val );
        private void Service_LoadedChanged( bool val ) => Loaded( val );

        private bool GetIsBusy() => isBusy ?? IsBusy;
        private bool GetIsLoaded() => isLoaded ?? IsLoaded;

        /// <inheritdoc/>
        public override Task SetParametersAsync( ParameterView parameters )
        {
            if ( parameters.TryGetValue( nameof( IsLoaded ), out bool newIsLoadedParameter ) )
            {
                if ( isLoadedParameter != newIsLoadedParameter )
                {
                    isLoaded = null; // use parameter instead of local value
                }
            }

            if ( parameters.TryGetValue( nameof( IsBusy ), out bool newIsBusyParameter ) )
            {
                if ( isBusyParameter != newIsBusyParameter )
                {
                    isBusy = null; // use parameter instead of local value
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
        public BusyLoadingService Service
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
        public bool IsLoaded
        {
            get => isLoaded ?? isLoadedParameter;
            set => isLoadedParameter = value;
        }

        /// <summary>
        /// Indicates whether the component should be covered with a busy screen
        /// </summary>
        [Parameter]
        public bool IsBusy
        {
            get => isBusy ?? isBusyParameter;
            set => isBusyParameter = value;
        }

        /// <summary>
        /// Occurs when IsLoaded state has changed
        /// </summary>
        [Parameter]
        public EventCallback<bool> IsLoadedChanged { get; set; }

        /// <summary>
        /// Occurs when IsBusy state has changed
        /// </summary>
        [Parameter]
        public EventCallback<bool> IsBusyChanged { get; set; }

        /// <inheritdoc/>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Busy indicator template
        /// </summary>
        [Parameter]
        public RenderFragment BusyIndicatorTemplate { get; set; }

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
        /// Spinner css size
        /// </summary>
        [Parameter]
        public string SpinnerSize { get; set; } = "64px";

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
        public bool IsFullScreen { get; set; }
        
        #endregion
    }
}
