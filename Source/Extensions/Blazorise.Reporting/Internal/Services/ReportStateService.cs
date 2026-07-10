#region Using directives
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportStateService
{
    #region Methods

    internal ReportState Capture(
        ReportDefinition definition,
        ReportMode mode,
        ReportPreviewFormat previewFormat,
        bool snapToGrid,
        ReportSelectionManager selectionManager,
        IReadOnlyList<ReportElementDefinition> clipboardElements,
        string clipboardBandId,
        bool canUndo,
        bool canRedo )
    {
        definition = ReportDefinitionHelper.EnsureDefinitionIds( definition );

        return new()
        {
            Definition = ReportContext.CloneDefinition( definition ),
            Mode = mode,
            PreviewFormat = previewFormat,
            SnapToGrid = snapToGrid,
            Selection = selectionManager.CaptureState( definition ),
            ClipboardElements = clipboardElements?.Select( ReportContext.CloneElement ).ToList() ?? [],
            ClipboardBandId = clipboardBandId,
            CanUndo = canUndo,
            CanRedo = canRedo,
        };
    }

    internal ReportState Apply(
        ReportState state,
        ReportDesignerInteractionState designerState,
        ReportSelectionManager selectionManager,
        System.Func<ReportDefinition> buildDeclarativeDefinition,
        out ReportDefinition definition,
        out List<ReportElementDefinition> clipboardElements,
        out string clipboardBandId )
    {
        ReportState nextState = ReportContext.CloneState( state );
        definition = ReportDefinitionHelper.EnsureDefinitionIds( nextState.Definition ?? buildDeclarativeDefinition() );
        designerState.SnapToGrid = nextState.SnapToGrid;
        clipboardElements = nextState.ClipboardElements?.Select( ReportContext.CloneElement ).ToList() ?? [];
        clipboardBandId = nextState.ClipboardBandId;
        selectionManager.ApplyState( definition, nextState.Selection );

        return nextState;
    }

    #endregion
}