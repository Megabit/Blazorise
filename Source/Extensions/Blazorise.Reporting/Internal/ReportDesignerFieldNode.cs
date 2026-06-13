#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerFieldNode
{
    #region Properties

    internal string Name { get; set; }

    internal string Path { get; set; }

    internal Type DataType { get; set; }

    internal bool IsCollection { get; set; }

    internal List<ReportDesignerFieldNode> Children { get; set; } = [];

    #endregion
}