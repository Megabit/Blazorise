#region Using directives
using System;
using System.Globalization;
using Blazorise.Utilities;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class BarDropdownToggle : Blazorise.BarDropdownToggle
{
    #region Methods

    protected override void BuildStyles( StyleBuilder builder )
    {
        base.BuildStyles( builder );

        if ( ParentBarDropdownState?.IsInlineDisplay == true )
        {
            double indentationLevel = Math.Max( ParentBarDropdownState.NestedIndex - 1, 0 ) * Indentation;
            string indentation = indentationLevel.ToString( CultureInfo.InvariantCulture );

            builder.Append( $"padding-left: calc(var(--ant-menu-item-padding-inline) + {indentation}rem)" );
        }
    }

    #endregion
}