using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazorise.Reporting;

internal sealed class ReportToolbarDockContext
{
    public ReportToolbarDockContext( IReadOnlyList<ReportToolbarDockPaneItem> panes, Func<string, Task> showPane )
    {
        Panes = panes;
        this.showPane = showPane;
    }

    public IReadOnlyList<ReportToolbarDockPaneItem> Panes { get; }

    public Task ShowPane( string paneName )
    {
        return showPane?.Invoke( paneName ) ?? Task.CompletedTask;
    }

    private readonly Func<string, Task> showPane;
}

internal sealed class ReportToolbarDockPaneItem
{
    public ReportToolbarDockPaneItem( string paneName, string caption )
    {
        PaneName = paneName;
        Caption = caption;
    }

    public string PaneName { get; }

    public string Caption { get; }
}