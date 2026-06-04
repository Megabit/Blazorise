using System.Collections.Generic;

namespace Blazorise.Reporting.Internal;

public sealed class ReportTreeNode
{
    public string Key { get; set; }

    public string Text { get; set; }

    public string Detail { get; set; }

    public ReportTreeNodeKind Kind { get; set; }

    public bool Selectable { get; set; }

    public bool Selected { get; set; }

    public bool Draggable { get; set; }

    public object Value { get; set; }

    public List<ReportTreeNode> Children { get; set; } = [];
}