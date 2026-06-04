using System.Threading.Tasks;

namespace Blazorise.Reporting;

internal interface IReportCommandExecutor
{
    Task ExecuteCommandAsync( ReportCommand command );

    bool CanExecuteCommand( ReportCommand command );

    bool IsCommandActive( ReportCommand command );
}

internal sealed class ReportToolbarContext
{
    public ReportToolbarContext( IReportCommandExecutor report )
    {
        Report = report;
    }

    public IReportCommandExecutor Report { get; }

    public Task ExecuteAsync( ReportCommand command )
    {
        return Report.ExecuteCommandAsync( command );
    }

    public bool CanExecute( ReportCommand command )
    {
        return Report.CanExecuteCommand( command );
    }

    public bool IsActive( ReportCommand command )
    {
        return Report.IsCommandActive( command );
    }
}