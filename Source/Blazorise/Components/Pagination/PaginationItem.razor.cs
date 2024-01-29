#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A container for page numbers links.
/// </summary>
public partial class PaginationItem : BaseComponent
{
    #region Members

    /// <summary>
    /// Holds the state of this pagination item.
    /// </summary>
    private PaginationItemState state = new();

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PaginationItem() );
        builder.Append( ClassProvider.PaginationItemActive( Active ) );
        builder.Append( ClassProvider.PaginationItemDisabled( Disabled ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the reference to the pagination item state object.
    /// </summary>
    protected PaginationItemState State => state;

    /// <summary>
    /// Gets the string representing the disabled state.
    /// </summary>
    protected string DisabledString => Disabled.ToString().ToLowerInvariant();

    /// <summary>
    /// Indicate the currently active page.
    /// </summary>
    [Parameter]
    public bool Active
    {
        get => state.Active;
        set
        {
            if ( value == state.Active )
                return;

            state = state with { Active = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Used for links that appear un-clickable.
    /// </summary>
    [Parameter]
    public bool Disabled
    {
        get => state.Disabled;
        set
        {
            if ( value == state.Disabled )
                return;

            state = state with { Disabled = value };

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="PaginationItem"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}