#region Using directives
using System;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Provides designer metadata and runtime loading behavior for a report data source type.
/// </summary>
public interface IReportDataSourceProvider
{
    #region Properties

    /// <summary>
    /// Stable provider type stored on report data source definitions.
    /// </summary>
    string Type { get; }

    /// <summary>
    /// User-facing provider name shown by designer dialogs.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Component type used by the designer to edit provider-specific settings. The component should expose a parameter named <c>Context</c> of type <see cref="ReportDataSourceProviderEditorContext" />.
    /// </summary>
    Type EditorComponentType { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Resolves the field schema exposed by the data source definition.
    /// </summary>
    /// <param name="definition">The data source definition to inspect.</param>
    /// <param name="cancellationToken">A token that cancels the schema request.</param>
    /// <returns>The field schema exposed by the data source.</returns>
    Task<ReportDataSourceSchema> GetSchemaAsync( ReportDataSourceDefinition definition, CancellationToken cancellationToken = default );

    /// <summary>
    /// Loads the data object consumed by the report renderer.
    /// </summary>
    /// <param name="definition">The data source definition to load.</param>
    /// <param name="context">Context supplied by the report renderer.</param>
    /// <param name="cancellationToken">A token that cancels the load request.</param>
    /// <returns>The loaded data and optional schema.</returns>
    Task<ReportDataSourceResult> LoadDataAsync( ReportDataSourceDefinition definition, ReportDataSourceLoadContext context, CancellationToken cancellationToken = default );

    #endregion
}