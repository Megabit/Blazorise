#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerDataSourceNode
{
    #region Properties

    internal string Name { get; set; }

    internal List<ReportDesignerFieldNode> Fields { get; set; } = [];

    #endregion
}