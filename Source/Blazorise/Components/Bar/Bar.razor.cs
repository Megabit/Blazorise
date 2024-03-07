#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// The <see cref="Bar"/> component is a wrapper that positions branding, navigation, and other elements into a concise header or sidebar.
/// </summary>
public partial class Bar : BaseComponent, IBreakpointActivator, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// Used to keep track of the breakpoint state for this component.
    /// </summary>
    private bool isBroken = true;

    /// <summary>
    /// Reference to the object that should be accessed through JSInterop.
    /// </summary>
    private DotNetObjectReference<BreakpointActivatorAdapter> dotNetObjectRef;

    /// <summary>
    /// Holds the state for this bar component.
    /// </summary>
    private BarState state = new()
    {
        Visible = true,
        Mode = BarMode.Horizontal,
        CollapseMode = BarCollapseMode.Hide,
        Breakpoint = Breakpoint.None,
        NavigationBreakpoint = Breakpoint.None,
        ThemeContrast = ThemeContrast.Light,
        Alignment = Alignment.Default,
    };

    /// <summary>
    /// Used to tell us that bar is initializing.
    /// </summary>
    private bool initial = true;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        if ( NavigationBreakpoint != Breakpoint.None )
            NavigationManager.LocationChanged += OnLocationChanged;

        return base.OnInitializedAsync();
    }

    /// <inheritdoc/>
    protected override async Task OnFirstAfterRenderAsync()
    {
        dotNetObjectRef ??= CreateDotNetObjectRef( new BreakpointActivatorAdapter( this ) );

        await JSBreakpointModule.RegisterBreakpoint( dotNetObjectRef, ElementId );

        if ( Mode != BarMode.Horizontal )
        {
            // Check if we need to collapse the Bar based on the current screen width against the breakpoint defined for this component.
            // This needs to be run to set the initial state, RegisterBreakpointComponent and OnBreakpoint will handle
            // additional changes to responsive breakpoints from there.
            isBroken = BreakpointActivatorAdapter.IsBroken( Breakpoint, await JSBreakpointModule.GetBreakpoint() );

            if ( isBroken )
            {
                initial = false;

                await Toggle();
            }
            else if ( initial )
            {
                initial = false;

                DirtyClasses();
                await InvokeAsync( StateHasChanged );
            }
        }

        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Bar( Mode ) );
        builder.Append( ClassProvider.BarInitial( Mode, initial && Mode != BarMode.Horizontal ) );
        builder.Append( ClassProvider.BarThemeContrast( Mode, ThemeContrast ), ThemeContrast != ThemeContrast.None );
        builder.Append( ClassProvider.BarBreakpoint( Mode, Breakpoint ), Breakpoint != Breakpoint.None );
        builder.Append( ClassProvider.FlexAlignment( Alignment ), Alignment != Alignment.Default );
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

    /// <inheritdoc/>
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
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            // make sure to unregister listener
            if ( Rendered )
            {
                var task = JSBreakpointModule.UnregisterBreakpoint( this );

                try
                {
                    await task;
                }
                catch when ( task.IsCanceled )
                {
                }
                catch ( Microsoft.JSInterop.JSDisconnectedException )
                {
                }

                DisposeDotNetObjectRef( dotNetObjectRef );
                dotNetObjectRef = null;
            }

            if ( NavigationBreakpoint != Breakpoint.None )
                NavigationManager.LocationChanged -= OnLocationChanged;
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// An event that fires when the navigation location has changed.
    /// </summary>
    /// <param name="sender">Object the fired the notification.</param>
    /// <param name="args">New location arguments.</param>
    private async void OnLocationChanged( object sender, LocationChangedEventArgs args )
    {
        // Collapse the bar automatically
        if ( Visible && BreakpointActivatorAdapter.IsBroken( NavigationBreakpoint, await JSBreakpointModule.GetBreakpoint() ) )
            await Toggle();
    }

    /// <summary>
    /// Hides all items except the one that is passed as a parameter.
    /// </summary>
    /// <param name="barItem">A bar item</param>
    /// <returns></returns>
    internal async Task HideAllExcept( BarItem barItem )
    {
        foreach ( var item in BarItems )
        {
            if ( item != barItem )
            {
                await item.HideDropdown();
            }
        }
    }

    /// <summary>
    /// Notifies the <see cref="Bar"/> of a new BarItem.
    /// </summary>
    /// <param name="barItem">Reference to the <see cref="BarItem"/> that is placed inside of this <see cref="Bar"/>.</param>
    internal void NotifyBarItemInitialized( BarItem barItem )
    {
        BarItems ??= new();
        if ( barItem is not null )
        {
            BarItems.Add( barItem );
        }
    }

    /// <summary>
    /// Notifies the <see cref="Bar"/> of a BarItem to be removed.
    /// </summary>
    /// <param name="barItem">Reference to the <see cref="BarItem"/> that is placed inside of this <see cref="Bar"/>.</param>
    internal void NotifyBarItemRemoved( BarItem barItem )
    {
        BarItems?.Remove( barItem );
    }

    #endregion

    #region Properties

    /// <summary>
    /// The Bar Items
    /// </summary>
    protected List<BarItem> BarItems { get; set; }

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
    /// Gets or sets the <see cref="IJSBreakpointModule"/> instance.
    /// </summary>
    [Inject] public IJSBreakpointModule JSBreakpointModule { get; set; }

    /// <summary>
    /// Injects the navigation manager.
    /// </summary>
    [Inject] protected NavigationManager NavigationManager { get; set; }

    /// <summary>
    /// Controls the state of toggler and the menu.
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

            DirtyClasses();

            VisibleChanged.InvokeAsync( value );
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
    /// Defines the preferred theme contrast for this <see cref="Bar"/> component.
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

    /// <summary>
    /// Cascaded theme settings.
    /// </summary>
    [CascadingParameter] public Theme Theme { get; set; }

    /// <summary>
    /// Cascaded layour header component.
    /// </summary>
    [CascadingParameter] protected LayoutHeader LayoutHeader { get; set; }

    /// <summary>
    /// Keeps a single bar item open at a time.
    /// </summary>
    [Parameter] public bool ToggleSingle { get; set; }

    #endregion
}