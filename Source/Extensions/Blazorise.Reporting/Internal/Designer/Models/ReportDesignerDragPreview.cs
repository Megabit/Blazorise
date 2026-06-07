namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerDragPreview
{
    #region Properties

    internal int SectionIndex { get; set; }

    internal ReportElementType ElementType { get; set; }

    internal string Text { get; set; }

    internal double X { get; set; }

    internal double Y { get; set; }

    internal double Width { get; set; }

    internal double Height { get; set; }

    #endregion
}