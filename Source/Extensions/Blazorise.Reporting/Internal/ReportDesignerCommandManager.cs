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

    internal async Task<ReportDesignerCommandResult> ExecuteAsync(
        ReportDesignerCommand command,
        ReportDefinition currentDefinition,
        Func<ReportDefinition, ReportState> captureState )
    {
        if ( command is null )
            return ReportDesignerCommandResult.Empty;

        var beforeState = command.TrackHistory ? captureState?.Invoke( currentDefinition ) : null;

        if ( command.Execute is not null )
            await command.Execute.Invoke();

        var definition = command.GetDefinition?.Invoke() ?? currentDefinition;

        if ( command.TrackHistory )
        {
            var afterState = captureState?.Invoke( definition );
            var action = new ReportStateHistoryAction( command.Name, beforeState, afterState );
            historyService.Record( action );
            afterState.CanUndo = historyService.CanUndo;
            afterState.CanRedo = historyService.CanRedo;
            designerState.State = ReportContext.CloneState( afterState );
        }
        else
        {
            designerState.State = captureState?.Invoke( definition );
        }

        return new()
        {
            Definition = definition,
            NotifyDefinitionChanged = command.NotifyDefinitionChanged,
        };
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

    #endregion
}