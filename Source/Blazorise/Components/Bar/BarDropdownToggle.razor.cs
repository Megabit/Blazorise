#region Using directives
using System;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// Toggles the visibility or collapse of <see cref="Bar"/> component.
/// </summary>
public partial class BarDropdownToggle : BaseComponent, ICloseActivator, IAsyncDisposable
{
    #region Members

    private BarDropdownState parentBarDropdownState;

    private BarItemState parentBarItemState;

    private bool jsRegistered;

    private DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef;

    private bool? disabled;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnFirstAfterRenderAsync()
    {
        dotNetObjectRef ??= CreateDotNetObjectRef( new CloseActivatorAdapter( this ) );

        return base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarDropdownToggle( ParentBarDropdownState.Mode, ParentBarDropdown?.IsBarDropdownSubmenu == true ) );
        builder.Append( ClassProvider.BarDropdownToggleDisabled( ParentBarDropdownState.Mode, ParentBarDropdown?.IsBarDropdownSubmenu == true, IsDisabled ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        base.BuildStyles( builder );

        builder.Append( $"padding-left: {( Indentation * ParentBarDropdownState.NestedIndex ).ToString( CultureInfo.InvariantCulture )}rem", ParentBarDropdownState.IsInlineDisplay );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            // make sure to unregister listener
            if ( jsRegistered )
            {
                jsRegistered = false;

                var task = JSClosableModule.Unregister( this );

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
            }

            DisposeDotNetObjectRef( dotNetObjectRef );
            dotNetObjectRef = null;
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Handles the button click event.
    /// </summary>
    /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
    /// <returns>Returns the awaitable task.</returns>
    protected async Task ClickHandler( MouseEventArgs eventArgs )
    {
        if ( IsDisabled )
            return;

        if ( ParentBarDropdown != null )
            await ParentBarDropdown.Toggle( ElementId );

        await Clicked.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Handler for @onkeydown event.
    /// </summary>
    /// <param name="eventArgs">Information about the keyboard down event.</param>
    /// <returns>Returns awaitable task</returns>
    protected Task KeyDownHandler( KeyboardEventArgs eventArgs )
    {
        if ( IsDisabled )
            return Task.CompletedTask;

        if ( ParentBarDropdown != null && eventArgs.Key == "Enter" )
            return ParentBarDropdown.Toggle( ElementId );

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<bool> IsSafeToClose( string elementId, CloseReason closeReason, bool isChildClicked )
    {
        return Task.FromResult( closeReason == CloseReason.EscapeClosing || ( ParentBarDropdown?.ShouldClose ?? true && ( elementId != ElementId && ParentBarDropdown?.SelectedBarDropdownElementId != ElementId && !isChildClicked ) ) );
    }

    /// <inheritdoc/>
    public Task Close( CloseReason closeReason )
    {
        if ( ParentBarDropdown != null )
            return ParentBarDropdown.Hide();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the visibility styles and JS interop states.
    /// </summary>
    /// <param name="visible">True if component is visible.</param>
    protected virtual void HandleVisibilityStyles( bool visible )
    {
        if ( visible )
        {
            jsRegistered = true;

            ExecuteAfterRender( async () =>
            {
                await JSClosableModule.Register( dotNetObjectRef, ElementRef );
            } );
        }
        else
        {
            jsRegistered = false;

            ExecuteAfterRender( async () =>
            {
                await JSClosableModule.Unregister( this );
            } );
        }

        DirtyClasses();
        DirtyStyles();
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Returns true if this BarDropdown should be disabled.
    /// </summary>
    protected internal bool IsDisabled => ( Disabled ?? ParentBarItem?.Disabled ) == true;

    /// <summary>
    /// Gets or sets the <see cref="IJSClosableModule"/> instance.
    /// </summary>
    [Inject] public IJSClosableModule JSClosableModule { get; set; }

    /// <summary>
    /// Determines how much left padding will be applied to the dropdown toggle. (in rem unit)
    /// </summary>
    [Parameter] public double Indentation { get; set; } = 1.5d;

    /// <summary>
    /// Makes the toggle element look inactive.
    /// </summary>
    [Parameter]
    public bool? Disabled
    {
        get => disabled;
        set
        {
            if ( disabled == value )
                return;

            disabled = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Occurs when the toggle button is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Gets or sets the parent dropdown state object.
    /// </summary>
    [CascadingParameter]
    public BarDropdownState ParentBarDropdownState
    {
        get => parentBarDropdownState;
        set
        {
            if ( parentBarDropdownState == value )
                return;

            parentBarDropdownState = value;

            if ( parentBarDropdownState.Visible && !( parentBarDropdownState.Mode == BarMode.VerticalInline && parentBarDropdownState.BarVisible ) )
            {
                HandleVisibilityStyles( true );
            }
            else
            {
                HandleVisibilityStyles( false );
            }
        }
    }

    /// <summary>
    /// Gets or sets the reference to the parent dropdown.
    /// </summary>
    [CascadingParameter] protected BarDropdown ParentBarDropdown { get; set; }

    /// <summary>
    /// Cascaded <see cref="BarItem"/> component in which this <see cref="BarDropdownToggle"/> is placed.
    /// </summary>
    [CascadingParameter] protected BarItem ParentBarItem { get; set; }

    /// <summary>
    /// Gets or sets the parent bar-item state object.
    /// </summary>
    [CascadingParameter]
    public BarItemState ParentBarItemState
    {
        get => parentBarItemState;
        set
        {
            if ( parentBarItemState == value )
                return;

            parentBarItemState = value;

            DirtyClasses();
            DirtyStyles();
        }
    }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="BarDropdownToggle"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}