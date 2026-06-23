using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazorise.Reporting;

internal sealed class ReportToolbarDockContext
{
    public ReportToolbarDockContext( IReadOnlyList<ReportToolbarDockPaneItem> panes, Func<string, bool> isPaneOpen, Func<string, bool, Task> setPaneOpen )
    {
        Panes = panes;
        this.isPaneOpen = isPaneOpen;
        this.setPaneOpen = setPaneOpen;
    }

    public IReadOnlyList<ReportToolbarDockPaneItem> Panes { get; }

    public bool IsPaneOpen( string paneName )
    {
        return isPaneOpen?.Invoke( paneName ) == true;
    }

    public Task SetPaneOpen( string paneName, bool open )
    {
        return setPaneOpen?.Invoke( paneName, open ) ?? Task.CompletedTask;
    }

    private readonly Func<string, bool> isPaneOpen;

    private readonly Func<string, bool, Task> setPaneOpen;
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