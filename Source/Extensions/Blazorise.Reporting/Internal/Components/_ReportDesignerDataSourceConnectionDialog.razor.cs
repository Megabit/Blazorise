#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Internal dialog used to add or update report data source connections.
/// </summary>
public partial class _ReportDesignerDataSourceConnectionDialog
{
    #region Members

    private const string NewDataSourceValue = "";

    private readonly List<IReportDataSourceProvider> providers = [];

    private readonly List<ReportDataSourceDefinition> dataSources = [];

    private Modal modalRef;

    private ReportDataSourceProviderEditorContext editorContext;

    private string selectedDataSourceId;

    private string selectedProviderType;

    private string name;

    #endregion

    #region Methods

    internal async Task ShowAsync( ReportDefinition definition, IEnumerable<IReportDataSourceProvider> providerOptions )
    {
        providers.Clear();
        providers.AddRange( providerOptions ?? [] );

        dataSources.Clear();
        dataSources.AddRange( definition?.DataSources ?? [] );

        selectedDataSourceId = NewDataSourceValue;
        selectedProviderType = providers.FirstOrDefault()?.Type;
        name = CreateUniqueDataSourceName();
        editorContext = CreateEditorContext( selectedProviderType, null );

        await modalRef.Show();
    }

    private Task CloseAsync()
    {
        return modalRef.Hide();
    }

    private async Task ConfirmAsync()
    {
        if ( !CanConfirm )
            return;

        ReportDataSourceDefinition existingDataSource = FindSelectedDataSource();
        Dictionary<string, object> settings = editorContext?.Settings?.ToDictionary( setting => setting.Key, setting => setting.Value, StringComparer.OrdinalIgnoreCase ) ?? [];
        bool connectionChanged = existingDataSource is null
            || !string.Equals( existingDataSource.ProviderType, selectedProviderType, StringComparison.OrdinalIgnoreCase )
            || !AreSettingsEqual( existingDataSource.Settings, settings );

        ReportDataSourceDefinition dataSource = new()
        {
            Id = existingDataSource?.Id ?? Guid.NewGuid().ToString( "N" ),
            Name = name?.Trim(),
            ProviderType = selectedProviderType,
            Data = existingDataSource?.Data,
            Schema = connectionChanged ? null : existingDataSource?.Schema,
            Settings = settings,
        };

        await Confirmed.InvokeAsync( dataSource );
        await modalRef.Hide();
    }

    private Task OnSelectedDataSourceChanged( string value )
    {
        selectedDataSourceId = value;

        ReportDataSourceDefinition dataSource = FindSelectedDataSource();

        if ( dataSource is null )
        {
            selectedProviderType = providers.FirstOrDefault()?.Type;
            name = CreateUniqueDataSourceName();
            editorContext = CreateEditorContext( selectedProviderType, null );
        }
        else
        {
            selectedProviderType = dataSource.ProviderType;
            name = dataSource.Name;
            editorContext = CreateEditorContext( selectedProviderType, dataSource.Settings );
        }

        return Task.CompletedTask;
    }

    private Task OnSelectedProviderChanged( string value )
    {
        selectedProviderType = value;
        editorContext = CreateEditorContext( selectedProviderType, null );

        return Task.CompletedTask;
    }

    private Task OnNameChanged( string value )
    {
        name = value;

        return Task.CompletedTask;
    }

    private ReportDataSourceProviderEditorContext CreateEditorContext( string providerType, IDictionary<string, object> settings )
    {
        return new( providerType, settings );
    }

    private IReportDataSourceProvider FindSelectedProvider()
    {
        return providers.FirstOrDefault( provider => string.Equals( provider.Type, selectedProviderType, StringComparison.OrdinalIgnoreCase ) );
    }

    private ReportDataSourceDefinition FindSelectedDataSource()
    {
        if ( string.IsNullOrWhiteSpace( selectedDataSourceId ) )
            return null;

        return dataSources.FirstOrDefault( dataSource => string.Equals( dataSource.Id, selectedDataSourceId, StringComparison.Ordinal ) );
    }

    private string CreateUniqueDataSourceName()
    {
        const string baseName = "DataSource";

        string candidate = baseName;
        int index = 2;

        while ( dataSources.Any( dataSource => string.Equals( dataSource.Name, candidate, StringComparison.OrdinalIgnoreCase ) ) )
        {
            candidate = $"{baseName}{index}";
            index++;
        }

        return candidate;
    }

    private static string GetDataSourceDisplayName( ReportDataSourceDefinition dataSource )
    {
        if ( dataSource is null )
            return null;

        return string.IsNullOrWhiteSpace( dataSource.Name )
            ? dataSource.ProviderType
            : $"{dataSource.Name} ({dataSource.ProviderType})";
    }

    private string GetProviderSettingsLabel()
    {
        IReportDataSourceProvider provider = FindSelectedProvider();

        return provider is null ? "Settings" : $"{provider.DisplayName} settings";
    }

    private static bool AreSettingsEqual( IDictionary<string, object> first, IDictionary<string, object> second )
    {
        first ??= new Dictionary<string, object>();
        second ??= new Dictionary<string, object>();

        if ( first.Count != second.Count )
            return false;

        foreach ( KeyValuePair<string, object> setting in first )
        {
            if ( !second.TryGetValue( setting.Key, out object value ) )
                return false;

            if ( !string.Equals(
                Convert.ToString( setting.Value, CultureInfo.InvariantCulture ),
                Convert.ToString( value, CultureInfo.InvariantCulture ),
                StringComparison.Ordinal ) )
                return false;
        }

        return true;
    }

    #endregion

    #region Properties

    private bool CanConfirm => providers.Count > 0
        && !string.IsNullOrWhiteSpace( selectedProviderType )
        && !string.IsNullOrWhiteSpace( name );

    /// <summary>
    /// Raised when a data source connection is confirmed.
    /// </summary>
    [Parameter] public EventCallback<ReportDataSourceDefinition> Confirmed { get; set; }

    private Type SelectedProviderEditorComponentType => FindSelectedProvider()?.EditorComponentType ?? typeof( _ReportDataSourceSettingsEditor );

    private Dictionary<string, object> ProviderEditorParameters => new()
    {
        [nameof( _ReportDataSourceSettingsEditor.Context )] = editorContext,
    };

    #endregion
}