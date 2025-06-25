#region Using directives
using System;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// A menu item for the <see cref="BarDropdownMenu"/> component.
/// </summary>
public partial class BarDropdownItem : BaseComponent
{
    #region Members

    private bool disabled;

    private BarDropdownState parentDropdownState;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarDropdownItem( ParentDropdownState.Mode ) );
        builder.Append( ClassProvider.BarDropdownItemDisabled( ParentDropdownState.Mode, Disabled ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        base.BuildStyles( builder );

        builder.Append( FormattableString.Invariant( $"padding-left: {( Indentation * ( ParentDropdownState.NestedIndex + 1d ) ).ToString( CultureInfo.InvariantCulture )}rem" ), ParentDropdownState.IsInlineDisplay );
    }

    /// <summary>
    /// Handles the item onclick event.
    /// </summary>
    /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task ClickHandler( MouseEventArgs eventArgs )
    {
        if ( !Disabled )
        {
            if ( ParentBarDropdown is not null && ParentDropdownState.Mode == BarMode.Horizontal )
            {
                if ( !ParentBarDropdown.WasJustToggled )
                    await ParentBarDropdown.Hide( true );
            }

            await Clicked.InvokeAsync( eventArgs );
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Cascaded parent <see cref="BarDropdown"/> state.
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
            DirtyStyles();
        }
    }

    /// <summary>
    /// Gets or sets the reference to the parent BarDropdown.
    /// </summary>
    [CascadingParameter] protected BarDropdown ParentBarDropdown { get; set; }

    /// <summary>
    /// Occurs when the item is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Specifies the URL of the page the link goes to.
    /// </summary>
    [Parameter] public string To { get; set; }

    /// <summary>
    /// The target attribute specifies where to open the linked document.
    /// </summary>
    [Parameter] public Target Target { get; set; } = Target.Default;

    /// <summary>
    /// URL matching behavior for a link.
    /// </summary>
    [Parameter] public Match Match { get; set; } = Match.All;

    /// <summary>
    /// Specify extra information about the link element.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Determines how much left padding will be applied to the dropdown item. (in rem unit)
    /// </summary>
    [Parameter] public double Indentation { get; set; } = 1.5d;

    /// <summary>
    /// Indicate the currently disabled item.
    /// </summary>
    [Parameter]
    public bool Disabled
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
    /// Specifies the content to be rendered inside this <see cref="BarDropdownItem"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}