#region Using directives
using System.Globalization;
using Blazorise.Utilities;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class BarDropdownItem : Blazorise.BarDropdownItem
{
    #region Methods

    protected override void BuildStyles( StyleBuilder builder )
    {
        base.BuildStyles( builder );

        if ( ParentDropdownState?.IsInlineDisplay == true )
        {
            double indentationLevel = ParentDropdownState.NestedIndex * Indentation;
            string indentation = indentationLevel.ToString( CultureInfo.InvariantCulture );

            builder.Append( $"padding-left: calc(var(--ant-menu-item-padding-inline) + {indentation}rem)" );
        }
    }

    #endregion
}