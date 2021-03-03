#region Using directives
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    /// <summary>
    /// The <see cref="Bar"/> component is a wrapper that positions branding, navigation, and other elements into a concise header or sidebar.
    /// </summary>
    public partial class Bar : BaseComponent, IBreakpointActivator
    {
        #region Members

        /// <summary>
        /// Used to keep track of the breakpoint state for this component.
        /// </summary>
        private bool isBroken;

        /// <summary>
        /// Reference to the object that should be accessed through JSInterop.
        /// </summary>
        private DotNetObjectReference<BreakpointActivatorAdapter> dotNetObjectRef;

        /// <summary>
        /// Holds the state for this bar component.
        /// </summary>
        private BarState state = new BarState
        {
            Visible = true,
            Mode = BarMode.Horizontal,
            CollapseMode = BarCollapseMode.Hide,
            Breakpoint = Breakpoint.None,
            NavigationBreakpoint = Breakpoint.None,
            ThemeContrast = ThemeContrast.Light,
            Alignment = Alignment.None,
            Background = Background.None,
        };

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override async Task OnInitializedAsync()
        {
            if ( NavigationBreakpoint != Breakpoint.None )
                NavigationManager.LocationChanged += OnLocationChanged;

            await base.OnInitializedAsync();
        }

        /// <inheritdoc/>
        protected override async Task OnFirstAfterRenderAsync()
        {
            dotNetObjectRef ??= CreateDotNetObjectRef( new BreakpointActivatorAdapter( this ) );

            _ = JSRunner.RegisterBreakpointComponent( dotNetObjectRef, ElementId );

            if ( Mode != BarMode.Horizontal )
            {
                // Check if we need to collapse the Bar based on the current screen width against the breakpoint defined for this component.
                // This needs to be run to set the inital state, RegisterBreakpointComponent and OnBreakpoint will handle
                // additional changes to responsive breakpoints from there.
                isBroken = BreakpointActivatorAdapter.IsBroken( Breakpoint, await JSRunner.GetBreakpoint() );

                if ( isBroken )
                {
                    await Toggle();
                }
            }

            await base.OnFirstAfterRenderAsync();
        }

        /// <inheritdoc/>
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

        /// <summary>
        /// Toggles the <see cref="Visible"/> state of the <see cref="Bar"/> component.
        /// </summary>
        internal Task Toggle()
        {
            Visible = !Visible;

            return InvokeAsync( StateHasChanged );
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

            return InvokeAsync( StateHasChanged );
        }

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                // make sure to unregister listener
                if ( Rendered )
                {
                    _ = JSRunner.UnregisterBreakpointComponent( this );

                    DisposeDotNetObjectRef( dotNetObjectRef );
                }

                if ( NavigationBreakpoint != Breakpoint.None )
                    NavigationManager.LocationChanged -= OnLocationChanged;
            }

            base.Dispose( disposing );
        }

        /// <summary>
        /// An event that fires when the navigation location has changed.
        /// </summary>
        /// <param name="sender">Object the fired the notification.</param>
        /// <param name="args">New location arguments.</param>
        private async void OnLocationChanged( object sender, LocationChangedEventArgs args )
        {
            // Collapse the bar automatically
            if ( Visible && BreakpointActivatorAdapter.IsBroken( NavigationBreakpoint, await JSRunner.GetBreakpoint() ) )
                await Toggle();
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        /// <summary>
        /// Gets the reference to the state object for this <see cref="Bar"/> component.
        /// </summary>
        protected BarState State => state;

        /// <summary>
        /// Gets the string representation of the <see cref="isBroken"/> flag.
        /// </summary>
        protected string BrokenStateString => isBroken.ToString().ToLower();

        /// <summary>
        /// Gets the string representation of the <see cref="CollapseMode"/>.
        /// </summary>
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
        /// Injects the navigation manager.
        /// </summary>
        [Inject] protected NavigationManager NavigationManager { get; set; }

        /// <summary>
        /// Controlls the state of toggler and the menu.
        /// </summary>
        [Parameter]
        public virtual bool Visible
        {
            get => state.Visible;
            set
            {
                // prevent bar from calling the same code multiple times
                if ( value == state.Visible )
                    return;

                state = state with { Visible = value };

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
            get => state.Breakpoint;
            set
            {
                state = state with { Breakpoint = value };

                DirtyClasses();
            }
        }


        /// <summary>
        /// Used for responsive collapsing after Navigation.
        /// </summary>
        [Parameter]
        public Breakpoint NavigationBreakpoint
        {
            get => state.NavigationBreakpoint;
            set
            {
                state = state with { NavigationBreakpoint = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines the prefered theme contrast for this <see cref="Bar"/> component.
        /// </summary>
        [Parameter]
        public ThemeContrast ThemeContrast
        {
            get => state.ThemeContrast;
            set
            {
                state = state with { ThemeContrast = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines the alignment within bar.
        /// </summary>
        [Parameter]
        public Alignment Alignment
        {
            get => state.Alignment;
            set
            {
                state = state with { Alignment = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the bar background color.
        /// </summary>
        [Parameter]
        public Background Background
        {
            get => state.Background;
            set
            {
                state = state with { Background = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines the orientation for the bar. Vertical is required when using as a Sidebar.
        /// </summary>
        [Parameter]
        public virtual BarMode Mode
        {
            get => state.Mode;
            set
            {
                if ( state.Mode == value )
                    return;

                state = state with { Mode = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines how the bar will be collapsed.
        /// </summary>
        [Parameter]
        public BarCollapseMode CollapseMode
        {
            get => state.CollapseMode;
            set
            {
                if ( state.CollapseMode == value )
                    return;

                state = state with { CollapseMode = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Bar"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
