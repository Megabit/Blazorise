#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Reporting;

internal interface IReportCommandExecutor
{
    Task ExecuteCommand( ReportCommand command );

    bool CanExecuteCommand( ReportCommand command );

    bool IsCommandActive( ReportCommand command );
}

internal sealed class ReportToolbarContext
{
    public ReportToolbarContext( IReportCommandExecutor report, Func<bool> statusBarVisible, Func<bool, Task> statusBarVisibilityChanged )
    {
        Report = report;
        this.statusBarVisible = statusBarVisible;
        this.statusBarVisibilityChanged = statusBarVisibilityChanged;
    }

    public IReportCommandExecutor Report { get; }

    public Task Execute( ReportCommand command )
    {
        return Report.ExecuteCommand( command );
    }

    public bool CanExecute( ReportCommand command )
    {
        return Report.CanExecuteCommand( command );
    }

    public bool IsActive( ReportCommand command )
    {
        return Report.IsCommandActive( command );
    }

    public bool StatusBarVisible
        => statusBarVisible?.Invoke() == true;

    public Task SetStatusBarVisible( bool visible )
        => statusBarVisibilityChanged?.Invoke( visible ) ?? Task.CompletedTask;

    private readonly Func<bool> statusBarVisible;

    private readonly Func<bool, Task> statusBarVisibilityChanged;
}