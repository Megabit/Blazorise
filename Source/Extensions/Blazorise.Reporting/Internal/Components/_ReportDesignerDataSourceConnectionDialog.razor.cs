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

    private bool? pendingConnectionCommit;

    private bool? connectionSucceeded;

    #endregion

    #region Methods

    internal async Task Show( ReportDefinition definition, IEnumerable<IReportDataSourceProvider> providerOptions )
    {
        ReportDataSourceConnectionDialogSession session = new();
        ModalInstanceOptions options = CreateReportModalOptions();
        options.Closing = session.OnClosing;

        await ShowReportModal<_ReportDesignerDataSourceConnectionDialog>( parameters =>
        {
            parameters.Add( nameof( Definition ), definition );
            parameters.Add( nameof( ProviderOptions ), providerOptions );
            parameters.Add( nameof( ConnectRequested ), ConnectRequested );
            parameters.Add( nameof( ConnectingChanged ), new Action<bool>( value => session.IsConnecting = value ) );
        }, options );
    }

    private Task Close()
    {
        return IsConnecting ? Task.CompletedTask : CloseReportModal();
    }

    private async Task Confirm()
    {
        if ( await RequestConnection( commit: true ) )
            await CloseReportModal();
    }

    private async Task TestConnection()
    {
        await RequestConnection( commit: false );
    }

    private async Task<bool> RequestConnection( bool commit )
    {
        if ( !CanConfirm || IsConnecting || ConnectRequested is null || ( !commit && !SupportsTestConnection ) )
            return false;

        ReportDataSourceDefinition dataSource = CreateDataSourceDefinition();
        connectionSucceeded = null;
        SetPendingConnection( commit );
        StateHasChanged();
        await Task.Yield();

        try
        {
            connectionSucceeded = await ConnectRequested( dataSource, commit );
            return connectionSucceeded.Value;
        }
        finally
        {
            SetPendingConnection( null );
        }
    }

    private void SetPendingConnection( bool? commit )
    {
        pendingConnectionCommit = commit;
        ConnectingChanged?.Invoke( commit.HasValue );
    }

    private ReportDataSourceDefinition CreateDataSourceDefinition()
    {
        ReportDataSourceDefinition existingDataSource = FindSelectedDataSource();
        Dictionary<string, object> settings = editorContext?.Settings?.ToDictionary( setting => setting.Key, setting => setting.Value, StringComparer.OrdinalIgnoreCase ) ?? [];
        bool connectionChanged = existingDataSource is null
            || !string.Equals( existingDataSource.ProviderType, selectedProviderType, StringComparison.OrdinalIgnoreCase )
            || !AreSettingsEqual( existingDataSource.Settings, settings );

        return new()
        {
            Id = existingDataSource?.Id ?? Guid.NewGuid().ToString( "N" ),
            Name = name?.Trim(),
            ProviderType = selectedProviderType,
            Data = existingDataSource?.Data,
            Schema = connectionChanged ? null : existingDataSource?.Schema,
            Settings = settings,
        };
    }

    private void SelectNewDataSource()
    {
        if ( IsConnecting )
            return;

        if ( !IsEditingDataSource && editorContext is not null )
            return;

        ResetConnectionResult();
        selectedDataSourceId = null;
        selectedProviderType = providers.FirstOrDefault()?.Type;
        name = CreateUniqueDataSourceName();
        editorContext = CreateEditorContext( selectedProviderType, null );
    }

    private void SelectExistingDataSource()
    {
        if ( IsConnecting )
            return;

        if ( IsEditingDataSource )
            return;

        ReportDataSourceDefinition dataSource = dataSources.FirstOrDefault();

        if ( dataSource is null )
            return;

        ResetConnectionResult();
        selectedDataSourceId = dataSource.Id;
        ApplyDataSource( dataSource );
    }

    private Task OnSelectedDataSourceChanged( string value )
    {
        if ( IsConnecting )
            return Task.CompletedTask;

        ResetConnectionResult();
        selectedDataSourceId = value;

        ReportDataSourceDefinition dataSource = FindSelectedDataSource();

        if ( dataSource is not null )
            ApplyDataSource( dataSource );

        return Task.CompletedTask;
    }

    private Task OnSelectedProviderChanged( string value )
    {
        if ( IsConnecting )
            return Task.CompletedTask;

        ResetConnectionResult();
        selectedProviderType = value;
        editorContext = CreateEditorContext( selectedProviderType, null );

        return Task.CompletedTask;
    }

    private Task OnNameChanged( string value )
    {
        if ( IsConnecting )
            return Task.CompletedTask;

        ResetConnectionResult();
        name = value;

        return Task.CompletedTask;
    }

    private ReportDataSourceProviderEditorContext CreateEditorContext( string providerType, IDictionary<string, object> settings )
    {
        return new( providerType, settings, ResetConnectionResult )
        {
            ResolveDisabled = () => IsConnecting,
        };
    }

    private void ResetConnectionResult()
    {
        if ( !connectionSucceeded.HasValue )
            return;

        connectionSucceeded = null;
        _ = InvokeAsync( StateHasChanged );
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

        return provider is null ? Localize( "Settings" ) : Localize( "{0} settings", provider.DisplayName );
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

    /// <inheritdoc />
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

    private bool IsConnecting => pendingConnectionCommit.HasValue;

    private bool IsTestingConnection => pendingConnectionCommit == false;

    private bool IsCommittingConnection => pendingConnectionCommit == true;

    private bool SupportsTestConnection => FindSelectedProvider()?.SupportsTestConnection == true;

    private bool HasConnectionResult => connectionSucceeded.HasValue;

    private Color ConnectionResultColor => connectionSucceeded == true ? Color.Success : Color.Danger;

    private string ConnectionResultMessage => connectionSucceeded == true
        ? Localize( "Connection succeeded." )
        : Localize( "Connection failed." );

    private bool IsEditingDataSource => !string.IsNullOrWhiteSpace( selectedDataSourceId );

    private string DialogTitle => IsEditingDataSource ? Localize( "Edit data source" ) : Localize( "Connect data source" );

    private string ConfirmCaption => IsEditingDataSource ? Localize( "Save changes" ) : Localize( "Connect" );

    private Type SelectedProviderEditorComponentType => FindSelectedProvider()?.EditorComponentType ?? typeof( _ReportDataSourceSettingsEditor );

    private Dictionary<string, object> ProviderEditorParameters => new()
    {
        [nameof( _ReportDataSourceSettingsEditor.Context )] = editorContext,
    };

    /// <summary>
    /// Report whose data source connections are edited.
    /// </summary>
    [Parameter] public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Providers offered for new connections.
    /// </summary>
    [Parameter] public IEnumerable<IReportDataSourceProvider> ProviderOptions { get; set; }

    /// <summary>
    /// Validates a data source connection and optionally commits it to the report definition.
    /// </summary>
    [Parameter] public Func<ReportDataSourceDefinition, bool, Task<bool>> ConnectRequested { get; set; }

    /// <summary>
    /// Notifies the modal owner when a connection request starts or finishes.
    /// </summary>
    [Parameter] public Action<bool> ConnectingChanged { get; set; }

    #endregion

}

internal sealed class ReportDataSourceConnectionDialogSession
{
    internal Task OnClosing( ModalClosingEventArgs eventArgs )
    {
        eventArgs.Cancel = IsConnecting;

        return Task.CompletedTask;
    }

    internal bool IsConnecting { get; set; }
}