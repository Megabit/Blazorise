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

    private readonly List<IReportDataSourceProvider> providers = [];

    private readonly List<ReportDataSourceDefinition> dataSources = [];

    private ReportDataSourceProviderEditorContext editorContext;

    private string selectedDataSourceId;

    private string selectedProviderType;

    private string name;

    #endregion

    #region Methods

    internal async Task Show( ReportDefinition definition, IEnumerable<IReportDataSourceProvider> providerOptions )
    {
        await ShowReportModal<_ReportDesignerDataSourceConnectionDialog>( parameters =>
        {
            parameters.Add( nameof( Definition ), definition );
            parameters.Add( nameof( ProviderOptions ), providerOptions );
            parameters.Add( nameof( Confirmed ), Confirmed );
        } );
    }

    private Task Close()
    {
        return CloseReportModal();
    }

    private async Task Confirm()
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
        await CloseReportModal();
    }

    private void SelectNewDataSource()
    {
        if ( !IsEditingDataSource && editorContext is not null )
            return;

        selectedDataSourceId = null;
        selectedProviderType = providers.FirstOrDefault()?.Type;
        name = CreateUniqueDataSourceName();
        editorContext = CreateEditorContext( selectedProviderType, null );
    }

    private void SelectExistingDataSource()
    {
        if ( IsEditingDataSource )
            return;

        ReportDataSourceDefinition dataSource = dataSources.FirstOrDefault();

        if ( dataSource is null )
            return;

        selectedDataSourceId = dataSource.Id;
        ApplyDataSource( dataSource );
    }

    private Task OnSelectedDataSourceChanged( string value )
    {
        selectedDataSourceId = value;

        ReportDataSourceDefinition dataSource = FindSelectedDataSource();

        if ( dataSource is not null )
            ApplyDataSource( dataSource );

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

    private void ApplyDataSource( ReportDataSourceDefinition dataSource )
    {
        selectedProviderType = dataSource.ProviderType;
        name = dataSource.Name;
        editorContext = CreateEditorContext( selectedProviderType, dataSource.Settings );
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

    protected override void OnInitialized()
    {
        providers.Clear();
        providers.AddRange( ProviderOptions ?? [] );

        dataSources.Clear();
        dataSources.AddRange( Definition?.DataSources ?? [] );

        SelectNewDataSource();
    }

    #endregion

    #region Properties

    private bool CanConfirm => providers.Count > 0
        && !string.IsNullOrWhiteSpace( selectedProviderType )
        && !string.IsNullOrWhiteSpace( name )
        && ( !IsEditingDataSource || FindSelectedDataSource() is not null );

    private bool IsEditingDataSource => !string.IsNullOrWhiteSpace( selectedDataSourceId );

    private string DialogTitle => IsEditingDataSource ? "Edit data source" : "Connect data source";

    private string ConfirmCaption => IsEditingDataSource ? "Save changes" : "Connect";

    private Type SelectedProviderEditorComponentType => FindSelectedProvider()?.EditorComponentType ?? typeof( _ReportDataSourceSettingsEditor );

    private Dictionary<string, object> ProviderEditorParameters => new()
    {
        [nameof( _ReportDataSourceSettingsEditor.Context )] = editorContext,
    };

    [Parameter] public ReportDefinition Definition { get; set; }

    [Parameter] public IEnumerable<IReportDataSourceProvider> ProviderOptions { get; set; }

    /// <summary>
    /// Raised when a data source connection is confirmed.
    /// </summary>
    [Parameter] public EventCallback<ReportDataSourceDefinition> Confirmed { get; set; }

    #endregion

}