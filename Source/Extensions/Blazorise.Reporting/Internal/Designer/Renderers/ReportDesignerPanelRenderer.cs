#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Blazorise;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDesignerPanelRenderer
{
    #region Methods

    internal static void Render( RenderTreeBuilder builder, object eventReceiver, IReadOnlyList<ReportDesignerPanelModule> modules, ReportDesignerPanelTab selectedTab, Action<ReportDesignerPanelTab> tabSelected )
    {
        if ( modules is null || modules.Count == 0 )
            return;

        var selectedModule = modules.FirstOrDefault( module => module.Tab == selectedTab ) ?? modules[0];

        RenderTabs( builder, eventReceiver, modules, selectedTab, tabSelected );

        builder.OpenElement( "div" );
        builder.Class( "b-report-designer-panel-body" );
        selectedModule.Render( builder );
        builder.CloseElement();
    }

    private static void RenderTabs( RenderTreeBuilder builder, object eventReceiver, IReadOnlyList<ReportDesignerPanelModule> modules, ReportDesignerPanelTab selectedTab, Action<ReportDesignerPanelTab> tabSelected )
    {
        builder.OpenComponent<Div>();
        builder.Class( "b-report-designer-tabs" );
        builder.Attribute( "ChildContent", (RenderFragment)( childBuilder =>
        {
            foreach ( var module in modules )
            {
                RenderTabButton( childBuilder, eventReceiver, module, selectedTab, tabSelected );
            }
        } ) );
        builder.CloseComponent();
    }

    private static void RenderTabButton( RenderTreeBuilder builder, object eventReceiver, ReportDesignerPanelModule module, ReportDesignerPanelTab selectedTab, Action<ReportDesignerPanelTab> tabSelected )
    {
        builder.OpenComponent<Button>();
        builder.Attribute( "Color", selectedTab == module.Tab ? Color.Primary : Color.Light );
        builder.Attribute( "Clicked", EventCallback.Factory.Create<MouseEventArgs>( eventReceiver, () => tabSelected( module.Tab ) ) );
        builder.Attribute( "ChildContent", (RenderFragment)( buttonBuilder => buttonBuilder.Content( module.Text ) ) );
        builder.CloseComponent();
    }

    #endregion
}