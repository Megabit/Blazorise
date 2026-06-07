#region Using directives
using System;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerPanelModule
{
    #region Constructors

    internal ReportDesignerPanelModule( ReportDesignerPanelTab tab, string text, Action<RenderTreeBuilder> render )
    {
        Tab = tab;
        Text = text;
        Render = render;
    }

    #endregion

    #region Properties

    internal ReportDesignerPanelTab Tab { get; }

    internal string Text { get; }

    internal Action<RenderTreeBuilder> Render { get; }

    #endregion
}