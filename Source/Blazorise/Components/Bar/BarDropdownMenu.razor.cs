#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Main container for a <see cref="BarDropdown"/> menu that can contain or or more <see cref="BarDropdownItem"/>'s.
/// </summary>
public partial class BarDropdownMenu : BaseComponent
{
    #region Members

    private BarDropdownState parentDropdownState;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="BarDropdownMenu"/> constructor.
    /// </summary>
    public BarDropdownMenu()
    {
        ContainerClassBuilder = new( BuildContainerClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarDropdownMenu( ParentDropdownState.Mode ) );
        builder.Append( ClassProvider.BarDropdownMenuVisible( ParentDropdownState.Mode, ParentDropdownState.Visible ) );
        builder.Append( ClassProvider.BarDropdownMenuRight( ParentDropdownState.Mode ), ParentDropdownState.RightAligned );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds the classnames for a menu.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    protected virtual void BuildContainerClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarDropdownMenuContainer( ParentDropdownState.Mode ) );
        builder.Append( ClassProvider.BarDropdownMenuRight( ParentDropdownState.Mode ), ParentDropdownState.RightAligned );
    }

    /// <inheritdoc/>
    internal protected override void DirtyClasses()
    {
        ContainerClassBuilder.Dirty();

        base.DirtyClasses();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the classnames for a dropdown-menu container.
    /// </summary>
    protected string ContainerClassNames => ContainerClassBuilder.Class;

    /// <summary>
    /// Dropdown-menu container class builder.
    /// </summary>
    protected ClassBuilder ContainerClassBuilder { get; private set; }

    /// <summary>
    /// Gets the string representation of visibility flag.
    /// </summary>
    protected string VisibleString => ParentDropdownState.Visible.ToString().ToLower();

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="BarDropdownMenu"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Cascaded <see cref="Dropdown"/> component state object.
    /// </summary>
    [CascadingParameter]
    protected BarDropdownState ParentDropdownState
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
    /// Gets or sets the reference to the parent <see cref="BarDropdown"/> component.
    /// </summary>
    [CascadingParameter] protected BarDropdown ParentBarDropdown { get; set; }

    #endregion
}