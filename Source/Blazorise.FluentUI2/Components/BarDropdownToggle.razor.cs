#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.FluentUI2.Components;

public partial class BarDropdownToggle : Blazorise.BarDropdownToggle
{
    #region Methods

    protected override void BuildStyles( StyleBuilder builder )
    {
        base.BuildStyles( builder );

        bool shouldAlignWithTopLevelItems = ParentBarDropdownState is
        {
            IsInlineDisplay: true,
            NestedIndex: 1,
        };

        builder.Append( "padding-left: var(--spacingHorizontalMNudge)", shouldAlignWithTopLevelItems );
    }

    #endregion
}