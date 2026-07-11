#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Describes a reusable formula-backed field available to report elements and text templates.
/// </summary>
public sealed class ReportFormulaFieldDefinition
{
    /// <summary>
    /// Stable identifier used by persisted report state.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    /// <summary>
    /// Formula field name shown in the field explorer and used by expressions.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Formula expression evaluated when the field is rendered.
    /// </summary>
    public string Formula { get; set; }
}