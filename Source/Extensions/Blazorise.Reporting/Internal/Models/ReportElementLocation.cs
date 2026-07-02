#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportElementLocation
{
    #region Properties

    internal int SectionIndex { get; set; }

    internal int ElementIndex { get; set; }

    internal ReportElementDefinition Element { get; set; }

    internal IList<ReportElementDefinition> OwnerElements { get; set; }

    internal ReportElementDefinition ParentTable { get; set; }

    internal ReportTableCellDefinition ParentCell { get; set; }

    internal bool IsTableCellElement => ParentCell is not null;

    #endregion
}