#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Describes a static text element placed on a report band.
/// </summary>
public sealed class ReportTextElementDefinition : ReportElementDefinition
{
    /// <inheritdoc />
    public override ReportElementType Type => ReportElementType.Text;

    /// <summary>
    /// Static text rendered by the element.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Optional data source used when resolving text expressions.
    /// </summary>
    public string DataSource { get; set; }
}