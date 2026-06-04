using System.Threading.Tasks;

namespace Blazorise.Reporting;

internal interface IReportCommandExecutor
{
    Task ExecuteCommandAsync( ReportCommand command );
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
}