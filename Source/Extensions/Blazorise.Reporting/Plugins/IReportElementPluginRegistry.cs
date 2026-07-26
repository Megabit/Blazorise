#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Provides registered report element plugins.
/// </summary>
public interface IReportElementPluginRegistry
{
    /// <summary>
    /// Gets all registered plugins.
    /// </summary>
    IReadOnlyList<IReportElementPlugin> Plugins { get; }

    /// <summary>
    /// Finds a plugin by its stable type name.
    /// </summary>
    IReportElementPlugin Find( string typeName );
}
