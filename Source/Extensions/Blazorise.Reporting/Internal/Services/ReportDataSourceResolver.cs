#region Using directives
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDataSourceResolver
{
    #region Members

    private readonly IReportDataSourceProviderRegistry providerRegistry;

    #endregion

    #region Constructors

    public ReportDataSourceResolver( IReportDataSourceProviderRegistry providerRegistry )
    {
        this.providerRegistry = providerRegistry;
    }

    #endregion

    #region Methods

    public async Task ResolveAsync( ReportDefinition definition, ReportDataSourceResolveOptions options, CancellationToken cancellationToken = default )
    {
        options ??= new();

        if ( definition?.DataSources is null || definition.DataSources.Count == 0 )
            return;

        if ( providerRegistry is null )
            throw new InvalidOperationException( "No report data source provider registry is available." );

        foreach ( ReportDataSourceDefinition dataSource in definition.DataSources )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if ( dataSource is null )
                continue;

            bool supplied = TryGetDataSource( options.DataSources, dataSource.Name, out object suppliedData );

            if ( supplied )
                dataSource.Data = suppliedData;

            IReportDataSourceProvider provider = providerRegistry.FindProvider( dataSource.ProviderType )
                ?? throw new InvalidOperationException( $"No report data source provider is registered for '{dataSource.ProviderType}'." );

            if ( dataSource.Data is null
                && options.DefaultData is not null
                && definition.DataSources.Count == 1
                && string.Equals( provider.Type, ObjectReportDataSourceProvider.ProviderType, StringComparison.OrdinalIgnoreCase ) )
            {
                dataSource.Data = options.DefaultData;
            }

            if ( options.LoadData && ShouldLoadDataSource( provider, dataSource, supplied ) )
            {
                ReportDataSourceResult result = await provider.LoadDataAsync( dataSource, new()
                {
                    DefaultData = options.DefaultData,
                    Parameters = options.Parameters is null
                        ? []
                        : new Dictionary<string, object>( options.Parameters, StringComparer.OrdinalIgnoreCase ),
                }, cancellationToken );

                if ( result is null )
                    throw new InvalidOperationException( $"The '{dataSource.Name}' report data source returned no result." );

                dataSource.Data = result.Data;
                dataSource.Schema = result.Schema ?? dataSource.Schema;
            }
            else if ( !options.LoadData && dataSource.Schema is null )
            {
                dataSource.Schema = await provider.GetSchemaAsync( dataSource, cancellationToken );
            }

            if ( options.RequireData && dataSource.Data is null )
                throw new InvalidOperationException( $"The '{dataSource.Name}' report data source returned no data." );
        }
    }

    private static bool ShouldLoadDataSource( IReportDataSourceProvider provider, ReportDataSourceDefinition dataSource, bool supplied )
    {
        return ( !supplied && dataSource.Data is null )
            || string.Equals( provider?.Type, DataSetReportDataSourceProvider.ProviderType, StringComparison.OrdinalIgnoreCase );
    }

    private static bool TryGetDataSource( IDictionary<string, object> dataSources, string name, out object data )
    {
        if ( dataSources is not null )
        {
            foreach ( KeyValuePair<string, object> item in dataSources )
            {
                if ( string.Equals( item.Key, name, StringComparison.OrdinalIgnoreCase ) )
                {
                    data = item.Value;
                    return true;
                }
            }
        }

        data = null;
        return false;
    }

    #endregion
}