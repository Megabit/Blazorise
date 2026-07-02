namespace Blazorise.Reporting.Internal;

internal sealed class ReportStateService
{
    #region Methods

    internal ReportState Capture(
        ReportDefinition definition,
        ReportStudioMode mode,
        ReportPreviewFormat previewFormat,
        bool snapToGrid,
        ReportSelectionManager selectionManager,
        ReportElementDefinition clipboardElement,
        string clipboardSectionId,
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
            ClipboardElement = ReportContext.CloneElement( clipboardElement ),
            ClipboardSectionId = clipboardSectionId,
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
        out ReportElementDefinition clipboardElement,
        out string clipboardSectionId )
    {
        ReportState nextState = ReportContext.CloneState( state );
        definition = ReportDefinitionHelper.EnsureDefinitionIds( nextState.Definition ?? buildDeclarativeDefinition() );
        designerState.SnapToGrid = nextState.SnapToGrid;
        clipboardElement = ReportContext.CloneElement( nextState.ClipboardElement );
        clipboardSectionId = nextState.ClipboardSectionId;
        selectionManager.ApplyState( definition, nextState.Selection );

        return nextState;
    }

    #endregion
}