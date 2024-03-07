#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Container for <see cref="BarLink"/> or <see cref="BarDropdown"/> components.
/// </summary>
public partial class BarItem : BaseComponent, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// Reference to the <see cref="Bar"/> state object.
    /// </summary>
    private BarState parentBarState;

    /// <summary>
    /// Holds the state for this <see cref="BarItem"/>.
    /// </summary>
    private BarItemState state = new()
    {
        Mode = BarMode.Horizontal,
    };

    /// <summary>
    /// Reference to the <see cref="BarDropdown"/> placed inside of this <see cref="BarItem"/>.
    /// </summary>
    private BarDropdown barDropdown;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        ParentBar?.NotifyBarItemInitialized( this );
    }

    /// <summary>
    /// The dropdown menu has been shown.
    /// </summary>
    /// <returns></returns>
    internal Task OnDropdownVisible()
    {
        if ( ParentBar.MenuToggleBehavior == BarMenuToggleBehavior.AllowSingleMenu )
        {
            ExecuteAfterRender( async () => await ParentBar.HideAllExcept( this ) );
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Hides the dropdown menu if it's visible.
    /// </summary>
    /// <returns></returns>
    internal protected Task HideDropdown()
    {
        if ( HasDropdown && barDropdown.Visible )
        {
            return barDropdown.Hide();
        }
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            if ( HasDropdown )
            {
                DirtyClasses();

                await InvokeAsync( StateHasChanged );
            }
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarItem( State.Mode, HasDropdown ) );
        builder.Append( ClassProvider.BarItemActive( State.Mode ), State.Active );
        builder.Append( ClassProvider.BarItemDisabled( State.Mode ), State.Disabled );
        builder.Append( ClassProvider.BarItemHasDropdown( State.Mode ), HasDropdown );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Notifies this <see cref="BarItem"/> that one of it's child component is a <see cref="BarDropdown"/>.
    /// </summary>
    /// <param name="barDropdown">Reference to the <see cref="BarDropdown"/> placed inside of this <see cref="BarItem"/>.</param>
    internal void NotifyBarDropdownInitialized( BarDropdown barDropdown )
    {
        this.barDropdown = barDropdown;

    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            ParentBar?.NotifyBarItemRemoved( this );
        }

        await base.DisposeAsync( disposing );
    }
    #endregion

    #region Properties

    /// <summary>
    /// Gets the reference to the state object for this <see cref="BarItem"/> component.
    /// </summary>
    protected BarItemState State => state;

    /// <summary>
    /// True if <see cref="BarDropdown"/> component is placed inside of this <see cref="BarItem"/>.
    /// </summary>
    protected bool HasDropdown => barDropdown is not null;

    /// <summary>
    /// Gets or sets the flag to indicate if <see cref="BarItem"/> is active, or focused.
    /// </summary>
    [Parameter]
    public bool Active
    {
        get => state.Active;
        set
        {
            state = state with { Active = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the disabled state to make <see cref="BarItem"/> inactive.
    /// </summary>
    [Parameter]
    public bool Disabled
    {
        get => state.Disabled;
        set
        {
            state = state with { Disabled = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Cascaded <see cref="Bar"/> component state object.
    /// </summary>
    [CascadingParameter]
    protected BarState ParentBarState
    {
        get => parentBarState;
        set
        {
            if ( parentBarState == value )
                return;

            parentBarState = value;

            state = state with { Mode = parentBarState.Mode, BarVisible = parentBarState.Visible };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the reference to the parent <see cref="Bar"/> component.
    /// </summary>
    [CascadingParameter] protected Bar ParentBar { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="BarItem"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}