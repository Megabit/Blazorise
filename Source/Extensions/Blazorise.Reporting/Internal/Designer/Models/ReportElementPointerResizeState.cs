#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportElementPointerResizeState
{
    #region Properties

    internal string ElementKey { get; set; }

    internal int SourceSectionIndex { get; set; }

    internal ReportElementResizeHandle Handle { get; set; }

    internal double OriginalX { get; set; }

    internal double OriginalY { get; set; }

    internal double OriginalWidth { get; set; }

    internal double OriginalHeight { get; set; }

    internal double StartClientX { get; set; }

    internal double StartClientY { get; set; }

    internal double TargetX { get; set; }

    internal double TargetY { get; set; }

    internal double TargetWidth { get; set; }

    internal double TargetHeight { get; set; }

    internal double MinimumHeight { get; set; }

    internal bool HasResized { get; set; }

    internal List<ReportElementPointerItemState> SelectedElements { get; set; } = [];

    #endregion
}