namespace Blazorise.Reporting.Internal;

internal sealed class ReportSectionPointerResizeState
{
    #region Properties

    internal int SectionIndex { get; set; }

    internal double OriginalHeight { get; set; }

    internal double TargetHeight { get; set; }

    internal double StartClientY { get; set; }

    #endregion
}