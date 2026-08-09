#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Describes a panel that groups child report elements.
/// </summary>
public sealed class ReportPanelElementDefinition : ReportElementDefinition
{
    /// <inheritdoc />
    public override ReportElementType Type => ReportElementType.Panel;

    /// <summary>
    /// Elements placed inside the panel.
    /// </summary>
    public List<ReportElementDefinition> Elements { get; set; } = [];
}