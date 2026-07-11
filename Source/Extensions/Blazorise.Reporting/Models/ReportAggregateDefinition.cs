#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Describes an aggregate operation applied to a report field element.
/// </summary>
public sealed class ReportAggregateDefinition
{
    /// <summary>
    /// Aggregate function used to calculate the field summary value.
    /// </summary>
    public ReportAggregateFunction Function { get; set; }
}