#region Using directives
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Reporting.Internal;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Provides schema and data loading support for object model report data sources.
/// </summary>
public sealed class ObjectReportDataSourceProvider : IReportDataSourceProvider
{
    #region Members

    /// <summary>
    /// Provider type used by object model data sources.
    /// </summary>
    public const string ProviderType = "object";

    #endregion

    #region Methods

    /// <inheritdoc />
    public Task<ReportDataSourceSchema> GetSchemaAsync( ReportDataSourceDefinition definition, CancellationToken cancellationToken = default )
    {
        return Task.FromResult( ReportDataSourceSchemaBuilder.FromData( definition?.Data ) );
    }

    /// <inheritdoc />
    public Task<ReportDataSourceResult> LoadDataAsync( ReportDataSourceDefinition definition, ReportDataSourceLoadContext context, CancellationToken cancellationToken = default )
    {
        return Task.FromResult( new ReportDataSourceResult
        {
            Data = definition?.Data,
            Schema = definition?.Schema ?? ReportDataSourceSchemaBuilder.FromData( definition?.Data ),
        } );
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    public string Type => ProviderType;

    /// <inheritdoc />
    public string DisplayName => "Object model";

    /// <inheritdoc />
    public System.Type EditorComponentType => null;

    #endregion
}