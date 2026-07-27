#region Using directives
using Blazorise.History;
using Blazorise.Reporting;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportStateHistoryAction : IHistoryAction<ReportDesignerState>
{
    private readonly ReportState beforeState;

    private readonly ReportState afterState;

    private readonly ReportDesignerRefreshTarget refreshTargets;

    internal ReportStateHistoryAction( string name, ReportState beforeState, ReportState afterState, ReportDesignerRefreshTarget refreshTargets )
    {
        Name = name;
        this.beforeState = beforeState;
        this.afterState = afterState;
        this.refreshTargets = refreshTargets;
    }

    public string Name { get; }

    public void Do( ReportDesignerState state )
    {
        state.State = ReportContext.CloneState( afterState );
        state.RefreshTargets = refreshTargets;
    }

    public void Undo( ReportDesignerState state )
    {
        state.State = ReportContext.CloneState( beforeState );
        state.RefreshTargets = refreshTargets;
    }
}