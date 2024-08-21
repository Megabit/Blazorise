#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// The dropdown menu, which can include bar items and dividers.
/// </summary>
public partial class BarDropdown : BaseComponent, IDisposable
{
    #region Members

    private BarItemState parentBarItemState;

    private BarDropdownState parentBarDropdownState;

    /// <summary>
    /// State object used to holds the dropdown state.
    /// </summary>
    private BarDropdownState state = new()
    {
        NestedIndex = 1
    };

    /// <summary>
    /// The direct BarDropdown child of this dropdown.
    /// </summary>
    private BarDropdown childBarDropdown;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarDropdown( State.Mode, IsBarDropdownSubmenu ) );
        builder.Append( ClassProvider.BarDropdownShow( State.Mode, State.Visible ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        if ( IsUnderFirstMenu )
        {
            ParentBarItem?.NotifyBarDropdownInitialized( this );
        }
        ParentBarDropdown?.NotifyChildDropdownInitialized( this );

        return base.OnInitializedAsync();
    }

    /// <inheritdoc/>
    protected override void OnAfterRender( bool firstRender )
    {
        WasJustToggled = false;

        base.OnAfterRender( firstRender );
    }

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var visibleChanged = parameters.TryGetValue<bool>( nameof( Visible ), out var paramVisible ) && Visible != paramVisible;

        await base.SetParametersAsync( parameters );

        if ( visibleChanged )
        {
            // This is needed for the two-way binding to work properly.
            // Otherwise the internal value would not be set in the right order.
            await SetVisibleState( paramVisible );
        }
    }

    /// <summary>
    /// Shows the dropdown menu.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Show()
    {
        if ( IsVisible )
            return;

        await SetVisibleState( true );

        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Hides the dropdown menu.
    /// </summary>
    /// <param name="hideAll">Indicates if we need to hide current dropdown menu and all its parent dropdown menus.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Hide( bool hideAll = false )
    {
        if ( !IsVisible )
            return;

        if ( ParentBarDropdown is not null && ( ParentBarDropdown.ShouldClose || hideAll ) )
            await ParentBarDropdown.Hide( hideAll );

        await SetVisibleState( false );

        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Toggles the visibility of the dropdown menu.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Toggle( string dropdownToggleElementId )
    {
        // Don't allow Toggle when menu is in a vertical "popout" style mode.
        // This will be handled by mouse over actions below.
        if ( ParentBarItemState is not null && ParentBarItemState.Mode != BarMode.Horizontal && !State.IsInlineDisplay )
            return;

        SetWasJustToggled( true );
        SetSelectedDropdownElementId( dropdownToggleElementId );

        await SetVisibleState( !state.Visible );

        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Handles the internal visibility states.
    /// </summary>
    /// <param name="visible">Visible state.</param>
    private async Task SetVisibleState( bool visible )
    {
        state = state with { Visible = visible };

        if ( visible )
        {
            if ( ParentBarItem is not null )
            {
                await ParentBarItem.OnDropdownVisible();
            }
        }

        await RaiseEvents( visible );
    }

    /// <summary>
    /// Fires all the events for this dropdown.
    /// </summary>
    /// <param name="visible">Visible state.</param>
    private Task RaiseEvents( bool visible )
    {
        return VisibleChanged.InvokeAsync( visible );
    }

    /// <summary>
    /// Sets the WasToggled Flag on the current Dropdown and every existing ParentDropdown.
    /// </summary>
    /// <param name="wasToggled"></param>
    internal void SetWasJustToggled( bool wasToggled )
    {
        WasJustToggled = wasToggled;
        ParentBarDropdown?.SetWasJustToggled( wasToggled );
    }

    /// <summary>
    /// Sets Selected Dropdown Toggle ElementId
    /// </summary>
    /// <param name="dropdownToggleElementId"></param>
    internal void SetSelectedDropdownElementId( string dropdownToggleElementId )
    {
        SelectedBarDropdownElementId = dropdownToggleElementId;
        if ( ParentBarDropdown is not null )
            ParentBarDropdown.SetSelectedDropdownElementId( dropdownToggleElementId );
    }

    /// <summary>
    /// Handles the onmouseenter event.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task OnMouseEnterHandler()
    {
        ShouldClose = false;

        if ( ParentBarItemState is not null && ParentBarItemState.Mode == BarMode.Horizontal || State.IsInlineDisplay )
            return Task.CompletedTask;

        return Show();
    }

    /// <summary>
    /// Handles the onmouseleave event.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task OnMouseLeaveHandler()
    {
        ShouldClose = true;

        if ( ParentBarItemState is not null && ParentBarItemState.Mode == BarMode.Horizontal || State.IsInlineDisplay )
            return Task.CompletedTask;

        return Hide();
    }

    /// <summary>
    /// Notifies the <see cref="BarDropdown"/> that it has a child BarDropdown component.
    /// </summary>
    /// <param name="barDropdown">Reference to the <see cref="BarDropdown"/> that is placed inside of this <see cref="BarDropdown"/>.</param>
    internal void NotifyChildDropdownInitialized( BarDropdown barDropdown )
    {
        if ( childBarDropdown is null )
            childBarDropdown = barDropdown;
    }

    /// <summary>
    /// Notifies the <see cref="BarDropdown"/> that it's a child BarDropdown component should be removed.
    /// </summary>
    /// <param name="barDropdown">Reference to the <see cref="BarDropdown"/> that is placed inside of this <see cref="BarDropdown"/>.</param>
    internal void NotifyChildDropdownRemoved( BarDropdown barDropdown )
    {
        childBarDropdown = null;
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            if ( ParentBarDropdown is not null )
            {
                ParentBarDropdown.NotifyChildDropdownRemoved( this );
            }
        }

        base.Dispose( disposing );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Whether the dropdown is the first menu in the bar.
    /// </summary>
    protected bool IsUnderFirstMenu
        => ParentBarDropdown is null;

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Keeps track whether the Dropdown is in a state where it should close.
    /// </summary>
    internal bool ShouldClose { get; set; } = false;

    /// <summary>
    /// Keeps track whether the Dropdown was just toggled, ignoring possible DropdownItem clicks which would otherwise close the dropdown.
    /// </summary>
    internal bool WasJustToggled { get; set; } = false;

    /// <summary>
    /// Indicates if the dropdown is visible.
    /// </summary>
    internal bool IsVisible => state.Visible;

    /// <summary>
    /// Gets the reference to the state object for this <see cref="BarDropdown"/> component.
    /// </summary>
    protected BarDropdownState State => state;

    /// <summary>
    /// Returns true if the BarDropdown is placed inside of another BarDropdown.
    /// </summary>
    protected internal bool IsBarDropdownSubmenu => ParentBarDropdown is not null;

    /// <summary>
    /// Returns true if this BarDropdown contains any child BarDropdown.
    /// </summary>
    protected internal bool HasSubmenu => childBarDropdown is not null;

    /// <summary>
    /// Gets the <see cref="Visible"/> flag represented as a string.
    /// </summary>
    protected string VisibleString => State.Visible.ToString().ToLower();

    /// <summary>
    /// Tracks the last BarDropdownToggle Element Id that acted.
    /// </summary>
    public string SelectedBarDropdownElementId { get; set; }

    /// <summary>
    /// Sets a value indicating whether the dropdown menu and all its child controls are visible.
    /// </summary>
    [Parameter] public bool Visible { get; set; }

    /// <summary>
    /// Occurs when the component visibility changes.
    /// </summary>
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>
    /// If true, a dropdown menu will be right aligned.
    /// </summary>
    [Parameter]
    public bool RightAligned
    {
        get => state.RightAligned;
        set
        {
            state = state with { RightAligned = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Cascaded <see cref="BarItem"/> component in which this <see cref="BarDropdown"/> is placed.
    /// </summary>
    [CascadingParameter] protected BarItem ParentBarItem { get; set; }

    /// <summary>
    /// Cascaded parent <see cref="BarItem"/> state.
    /// </summary>
    [CascadingParameter]
    protected BarItemState ParentBarItemState
    {
        get => parentBarItemState;
        set
        {
            if ( parentBarItemState == value )
                return;

            parentBarItemState = value;

            state = state with { Mode = parentBarItemState.Mode, BarVisible = parentBarItemState.BarVisible };

            if ( !state.BarVisible )
            {
                state = state with { Visible = false };
            }

            DirtyClasses();
        }
    }

    /// <summary>
    /// Cascaded parent <see cref="BarDropdown"/> state.
    /// </summary>
    [CascadingParameter]
    protected BarDropdownState ParentBarDropdownState
    {
        get => parentBarDropdownState;
        set
        {
            if ( parentBarDropdownState == value )
                return;

            parentBarDropdownState = value;

            state = state with { NestedIndex = parentBarDropdownState.NestedIndex + 1 };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the cascaded parent BarDropdown component.
    /// </summary>
    [CascadingParameter] protected BarDropdown ParentBarDropdown { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="BarDropdownItem"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}