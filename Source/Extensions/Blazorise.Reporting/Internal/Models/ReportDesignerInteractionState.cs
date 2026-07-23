#region Using directives
using System;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerInteractionState
{
    #region Properties

    public ReportContextMenuState ContextMenu { get; set; }

    public string EditingElementKey { get; set; }

    public string SuppressNextElementClickKey { get; set; }

    public DateTime SuppressSelectionClickUntil { get; set; }

    public ReportDesignerDragKind DraggedKind { get; set; }

    public string DraggedDataSourceName { get; set; }

    public string DraggedFieldName { get; set; }

    public ReportElementType? DraggedElementType { get; set; }

    public string DraggedElementText { get; set; }

    public string DraggedElementKey { get; set; }

    public ReportElementDefinition DraggedElement { get; set; }

    public ReportDesignerDragPreview DragPreview { get; set; }

    public ReportDesignerSelectionBox SelectionBox { get; set; }

    public ReportElementPointerDragState ElementPointerDrag { get; set; }

    public ReportElementPointerResizeState ElementPointerResize { get; set; }

    public ReportTablePointerResizeState TablePointerResize { get; set; }

    public ReportSectionPointerResizeState SectionPointerResize { get; set; }

    public DateTime LastDragPreviewRenderTime { get; set; }

    public DateTime LastSelectionBoxRenderTime { get; set; }

    #endregion
}