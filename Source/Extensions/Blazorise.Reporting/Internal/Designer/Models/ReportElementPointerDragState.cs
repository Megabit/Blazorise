#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportElementPointerDragState
{
    #region Properties

    internal string ElementKey { get; set; }

    internal int SourceSectionIndex { get; set; }

    internal int TargetSectionIndex { get; set; }

    internal double OriginalX { get; set; }

    internal double OriginalY { get; set; }

    internal double StartClientX { get; set; }

    internal double StartClientY { get; set; }

    internal double TargetX { get; set; }

    internal double TargetY { get; set; }

    internal bool HasMoved { get; set; }

    internal List<ReportElementPointerItemState> SelectedElements { get; set; } = [];

    #endregion
}