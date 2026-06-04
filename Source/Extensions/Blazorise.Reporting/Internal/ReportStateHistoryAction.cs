using Blazorise.History;
using Blazorise.Reporting;

namespace Blazorise.Reporting.Internal;

internal sealed class ReportStateHistoryAction : IHistoryAction<ReportDesignerState>
{
    private readonly ReportState beforeState;

    private readonly ReportState afterState;

    public ReportStateHistoryAction( string name, ReportState beforeState, ReportState afterState )
    {
        Name = name;
        this.beforeState = beforeState;
        this.afterState = afterState;
    }

    public string Name { get; }

    public void Do( ReportDesignerState state )
    {
        state.State = ReportContext.CloneState( afterState );
    }

    public void Undo( ReportDesignerState state )
    {
        state.State = ReportContext.CloneState( beforeState );
    }
}