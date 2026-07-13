#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportSelectedElementContext
{
    internal string ElementKey { get; set; }

    internal int SectionIndex { get; set; }

    internal ReportElementDefinition Element { get; set; }

    internal IList<ReportElementDefinition> OwnerElements { get; set; }

    internal double OwnerOffsetX { get; set; }

    internal double OwnerOffsetY { get; set; }

    internal ReportPanelElementDefinition ParentPanel { get; set; }
}