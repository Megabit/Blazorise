#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

public sealed class ReportDesignerDataSourceNode
{
    #region Constructors

    internal ReportDesignerDataSourceNode()
    {
    }

    #endregion

    #region Properties

    internal string Name { get; set; }

    internal string BindingName { get; set; }

    internal List<ReportDesignerFieldNode> Fields { get; set; } = [];

    #endregion
}