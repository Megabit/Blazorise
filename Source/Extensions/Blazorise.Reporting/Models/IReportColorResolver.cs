#region Using directives
using System;
using System.ComponentModel;
using System.Globalization;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Resolves report colors into RGB and alpha values.
/// </summary>
public interface IReportColorResolver
{
    /// <summary>
    /// Resolves a report color into concrete red, green, blue, and alpha components.
    /// </summary>
    /// <param name="color">Report color to resolve.</param>
    /// <returns>Resolved color components.</returns>
    ReportResolvedColor Resolve( ReportColor color );
}