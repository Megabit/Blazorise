#region Using directives
using System;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Main container for a <see cref="Dropdown"/> menu that can contain or or more <see cref="DropdownItem"/>'s.
/// </summary>
public partial class DropdownMenu : BaseComponent, IDisposable
{
    #region Members

    private DropdownState parentDropdownState;

    private string maxMenuHeight;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if ( ParentDropdown is not null )
        {
            ParentDropdown.NotifyDropdownMenuInitialized( this );
        }

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            if ( ParentDropdown is not null )
            {
                ParentDropdown.NotifyDropdownMenuRemoved( this );
            }
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DropdownMenu() );
        builder.Append( ClassProvider.DropdownMenuScrollable(), MaxMenuHeight is not null );
        builder.Append( ClassProvider.DropdownMenuVisible( ParentDropdownState.Visible ) );
        builder.Append( ClassProvider.DropdownMenuRight(), ParentDropdownState.RightAligned );
        builder.Append( ClassProvider.DropdownMenuPositionStrategy( ParentDropdown.PositionStrategy ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( MaxMenuHeight is not null )
            builder.Append( $"--dropdown-list-menu-max-height: {MaxMenuHeight};" );

        base.BuildStyles( builder );
    }

    /// <summary>
    /// Handles the dropdown visibility state change.
    /// </summary>
    /// <param name="visible">Visibility flag.</param>
    internal protected virtual void OnVisibleChanged( bool visible )
    {
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Container"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Cascaded <see cref="Dropdown"/> component state object.
    /// </summary>
    [CascadingParameter]
    protected DropdownState ParentDropdownState
    {
        get => parentDropdownState;
        set
        {
            if ( parentDropdownState == value )
                return;

            parentDropdownState = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Sets the maximum height of the dropdown menu.
    /// </summary>
    [Parameter]
    public string MaxMenuHeight
    {
        get => maxMenuHeight;
        set
        {
            maxMenuHeight = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the reference to the parent <see cref="Dropdown"/> component.
    /// </summary>
    [CascadingParameter] protected Dropdown ParentDropdown { get; set; }

    #endregion
}