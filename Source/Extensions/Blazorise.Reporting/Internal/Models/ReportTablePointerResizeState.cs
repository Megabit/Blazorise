namespace Blazorise.Reporting.Internal;

internal sealed class ReportTablePointerResizeState
{
    #region Properties

    internal string TableKey { get; set; }

    internal string CellKey { get; set; }

    internal int SectionIndex { get; set; }

    internal ReportTableResizeKind Kind { get; set; }

    internal int Index { get; set; }

    internal double OriginalSize { get; set; }

    internal double AdjacentOriginalSize { get; set; }

    internal double TargetSize { get; set; }

    internal double StartClientX { get; set; }

    internal double StartClientY { get; set; }

    internal bool SnapToGrid { get; set; }

    internal bool ResizesTable { get; set; }

    internal bool HasResized { get; set; }

    #endregion
}