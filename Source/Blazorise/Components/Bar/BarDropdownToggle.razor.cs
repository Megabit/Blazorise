#region Using directives
using System;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Extensions;
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
public partial class BarDropdownToggle : BaseLinkComponent, ICloseActivator, IAsyncDisposable
{
    #region Members

    private bool jsRegistered;

    private DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef;

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
        builder.Append( ClassProvider.BarDropdownToggleIcon( IsToggleIconVisible ) );

        if ( To != null )
        {
            builder.Append( ClassProvider.BarLink( ParentBarDropdownState.Mode ) );
            builder.Append( ClassProvider.LinkActive( Active ) );
        }

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
        if ( disposing )
        {
            if ( Rendered )
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
                    catch ( JSDisconnectedException )
                    {
                    }
                }

                DisposeDotNetObjectRef( dotNetObjectRef );
                dotNetObjectRef = null;
            }
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

        if ( ParentBarDropdown is not null )
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

        if ( ParentBarDropdown is not null && eventArgs.Key == "Enter" )
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
        if ( ParentBarDropdown is not null )
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
    protected internal bool IsDisabled => paramDisabled.GetValueOrDefault( ParentBarItem?.Disabled ?? false );

    /// <summary>
    /// Should the toggle icon be drawn
    /// </summary>
    protected bool IsToggleIconVisible => ToggleIconVisible.GetValueOrDefault( Theme?.BarOptions?.Dropdown?.ToggleIconVisible ?? true );

    /// <summary>
    /// Indicates whether the current instance is acting as a link to another object.
    /// </summary>
    protected bool IsActingAsLink => paramTo.Defined && paramTo.Value is not null;

    /// <summary>
    /// Gets or sets the <see cref="IJSClosableModule"/> instance.
    /// </summary>
    [Inject] public IJSClosableModule JSClosableModule { get; set; }

    /// <summary>
    /// Determines how much left padding will be applied to the dropdown toggle. (in rem unit)
    /// </summary>
    [Parameter] public double Indentation { get; set; } = 1.5d;

    /// <summary>
    /// Gets or sets a value indicating whether the dropdown toggle icon is visible.
    /// </summary>
    /// <value>
    /// <c>true</c> if [show toggle]; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>Default: True</remarks>
    [Parameter] public bool? ToggleIconVisible { get; set; }

    /// <summary>
    /// Gets or sets the parent dropdown state object.
    /// </summary>
    [CascadingParameter]
    public BarDropdownState ParentBarDropdownState
    {
        get;
        set
        {
            if ( field == value )
                return;

            field = value;

            if ( field.Visible && !( field.Mode == BarMode.VerticalInline && field.BarVisible ) )
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
        get;
        set
        {
            if ( field == value )
                return;

            field = value;

            DirtyClasses();
            DirtyStyles();
        }
    }

    /// <summary>
    /// The applied theme.
    /// </summary>
    [CascadingParameter] protected Theme Theme { get; set; }

    #endregion
}