#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.History;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerCommandManager
{
    #region Members

    private readonly ReportDesignerState designerState = new();

    private readonly HistoryManager<ReportDesignerState> historyService = new();

    #endregion

    #region Methods

    internal void Clear()
    {
        historyService.Clear();
    }

    internal async Task<ReportDefinition> Execute(
        ReportDesignerCommand command,
        ReportDefinition currentDefinition,
        Func<ReportDefinition, ReportState> captureState )
    {
        ReportState beforeState = command.TrackHistory ? captureState( currentDefinition ) : null;

        await command.Execute();

        ReportDefinition definition = command.GetDefinition?.Invoke() ?? currentDefinition;

        if ( command.TrackHistory )
        {
            ReportState afterState = captureState( definition );
            historyService.Record( new ReportStateHistoryAction( command.Name, beforeState, afterState ) );
            afterState.CanUndo = historyService.CanUndo;
            afterState.CanRedo = historyService.CanRedo;
            designerState.State = ReportContext.CloneState( afterState );
        }
        else
        {
            designerState.State = captureState( definition );
        }

        return definition;
    }

    internal ReportState Undo()
    {
        if ( !historyService.CanUndo )
            return null;

        historyService.Undo( designerState );

        return designerState.State;
    }

    internal ReportState Redo()
    {
        if ( !historyService.CanRedo )
            return null;

        historyService.Redo( designerState );

        return designerState.State;
    }

    internal void SetState( ReportState state )
    {
        designerState.State = ReportContext.CloneState( state );
    }

    #endregion

    #region Properties

    internal bool CanUndo => historyService.CanUndo;

    internal bool CanRedo => historyService.CanRedo;

    internal ReportState State => designerState.State;

    #endregion
}