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

        ReportState beforeState = command.TrackHistory ? captureState?.Invoke( currentDefinition ) : null;

        if ( command.Execute is not null )
            await command.Execute.Invoke();

        ReportDefinition definition = command.GetDefinition?.Invoke() ?? currentDefinition;

        if ( command.TrackHistory )
        {
            ReportState afterState = captureState?.Invoke( definition );
            RecordStateChange( command.Name, beforeState, afterState );
        }
        else
        {
            designerState.State = captureState?.Invoke( definition );
        }

        return new()
        {
            Definition = definition,
            NotifyDefinitionChanged = command.NotifyDefinitionChanged,
            RefreshSurface = command.RefreshSurface,
        };
    }

    internal void RecordStateChange( string name, ReportState beforeState, ReportState afterState )
    {
        if ( beforeState is null || afterState is null )
            return;

        ReportStateHistoryAction action = new( name, beforeState, afterState );
        historyService.Record( action );
        afterState.CanUndo = historyService.CanUndo;
        afterState.CanRedo = historyService.CanRedo;
        designerState.State = ReportContext.CloneState( afterState );
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