#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Small text that can appear next to the <see cref="DropdownItem"/>.
/// </summary>
public partial class BarLabel : BaseComponent
{
    #region Members

    private BarState parentBarState;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarLabel( ParentBarState?.Mode ?? BarMode.Horizontal ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

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

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the content rendered inside the bar label.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
