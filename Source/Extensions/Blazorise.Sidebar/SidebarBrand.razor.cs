﻿#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Sidebar;

public partial class SidebarBrand : BaseComponent
{
    #region Members

    #endregion

    #region Methods

    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "sidebar-brand" );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}