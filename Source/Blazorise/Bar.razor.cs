#region Using directives
using System.Threading.Tasks;
using Blazorise.Stores;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    public partial class Bar : BaseComponent, IBreakpointActivator
    {
        #region Members

        /// <summary>
        /// Used to keep track of the breakpoint state for this component.
        /// </summary>
        private bool isBroken;

        private Breakpoint breakpoint = Breakpoint.None;

        private Breakpoint navigationBreakpoint = Breakpoint.None;

        private ThemeContrast themeContrast = ThemeContrast.Light;

        private Alignment alignment = Alignment.None;

        private Background background = Background.None;

        private DotNetObjectReference<BreakpointActivatorAdapter> dotNetObjectRef;

        private BarStore store = new BarStore
        {
            Visible = true,
            Mode = BarMode.Horizontal,
            CollapseMode = BarCollapseMode.Hide
        };

        #endregion

        #region Methods

        protected override async Task OnInitializedAsync()
        {
            if ( NavigationBreakpoint != Breakpoint.None )
                NavigationManager.LocationChanged += OnLocationChanged;

            await base.OnInitializedAsync();
        }

        protected override async Task OnFirstAfterRenderAsync()
        {
            dotNetObjectRef ??= JSRunner.CreateDotNetObjectRef( new BreakpointActivatorAdapter( this ) );

            if ( Rendered )
            {
                _ = JSRunner.RegisterBreakpointComponent( dotNetObjectRef, ElementId );

                if ( Mode != BarMode.Horizontal )
                {
                    // Check if we need to collapse the Bar based on the current screen width against the breakpoint defined for this component.
                    // This needs to be run to set the inital state, RegisterBreakpointComponent and OnBreakpoint will handle
                    // additional changes to responsive breakpoints from there.
                    isBroken = BreakpointActivatorAdapter.IsBroken( Breakpoint, await JSRunner.GetBreakpoint() );

                    if ( Visible == isBroken )
                    {
                        Visible = !isBroken;
                        StateHasChanged();
                    }
                }
            }

            await base.OnFirstAfterRenderAsync();
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Bar() );
            builder.Append( ClassProvider.BarBackground( Background ), Background != Background.None );
            builder.Append( ClassProvider.BarThemeContrast( ThemeContrast ), ThemeContrast != ThemeContrast.None );
            builder.Append( ClassProvider.BarBreakpoint( Breakpoint ), Breakpoint != Breakpoint.None );
            builder.Append( ClassProvider.FlexAlignment( Alignment ), Alignment != Alignment.None );
            builder.Append( ClassProvider.BarMode( Mode ) );

            base.BuildClasses( builder );
        }

        internal void Toggle()
        {
            Visible = !Visible;

            StateHasChanged();
        }

        public Task OnBreakpoint( bool broken )
        {
            // If the breakpoint state has changed, we need to toggle the visibility of this component.
            // broken = true, hide the component
            // broken = false, show the component
            if ( isBroken == broken )
                return Task.CompletedTask;

            isBroken = broken;
            Visible = !isBroken;

            StateHasChanged();

            return Task.CompletedTask;
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                // make sure to unregister listener
                if ( Rendered )
                {
                    _ = JSRunner.UnregisterBreakpointComponent( this );

                    JSRunner.DisposeDotNetObjectRef( dotNetObjectRef );
                }

                if ( NavigationBreakpoint != Breakpoint.None )
                    NavigationManager.LocationChanged -= OnLocationChanged;
            }

            base.Dispose( disposing );
        }

        private async void OnLocationChanged( object sender, LocationChangedEventArgs args )
        {
            // Collapse the bar automatically
            if ( Visible && BreakpointActivatorAdapter.IsBroken( NavigationBreakpoint, await JSRunner.GetBreakpoint() ) )
                Toggle();
        }

        #endregion

        #region Properties

        protected BarStore Store => store;

        protected string BrokenStateString => isBroken.ToString().ToLower();

        protected string CollapseModeString
        {
            get
            {
                if ( Visible )
                    return null;

                return ClassProvider.ToBarCollapsedMode( CollapseMode );
            }
        }

        /// <summary>
        /// Controlls the state of toggler and the menu.
        /// </summary>
        [Parameter]
        public virtual bool Visible
        {
            get => store.Visible;
            set
            {
                // prevent bar from calling the same code multiple times
                if ( value == store.Visible )
                    return;

                store.Visible = value;
                VisibleChanged.InvokeAsync( value );

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs when the bar visibility changes.
        /// </summary>
        [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

        /// <summary>
        /// Used for responsive collapsing.
        /// </summary>
        [Parameter]
        public Breakpoint Breakpoint
        {
            get => breakpoint;
            set
            {
                breakpoint = value;

                DirtyClasses();
            }
        }


        /// <summary>
        /// Used for responsive collapsing after Navigation
        /// </summary>
        [Parameter]
        public Breakpoint NavigationBreakpoint
        {
            get => navigationBreakpoint;
            set
            {
                navigationBreakpoint = value;

                DirtyClasses();
            }
        }

        [Parameter]
        public ThemeContrast ThemeContrast
        {
            get => themeContrast;
            set
            {
                themeContrast = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines the alignment within bar.
        /// </summary>
        [Parameter]
        public Alignment Alignment
        {
            get => alignment;
            set
            {
                alignment = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the bar background color.
        /// </summary>
        [Parameter]
        public Background Background
        {
            get => background;
            set
            {
                background = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines the Orientation for the bar. Vertical is required when using inside Sidebar.
        /// </summary>
        [Parameter]
        public virtual BarMode Mode
        {
            get => store.Mode;
            set
            {
                if ( store.Mode == value )
                    return;

                store.Mode = value;

                DirtyClasses();
            }
        }

        [Parameter]
        public BarCollapseMode CollapseMode
        {
            get => store.CollapseMode;
            set
            {
                if ( store.CollapseMode == value )
                    return;

                store.CollapseMode = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Inject] protected NavigationManager NavigationManager { get; set; }

        #endregion
    }
}
