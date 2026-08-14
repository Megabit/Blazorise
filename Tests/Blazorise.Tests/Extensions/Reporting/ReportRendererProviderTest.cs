#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Pdf;
using Blazorise.Reporting;
using Blazorise.Reporting.DataSources.Csv;
using Blazorise.Reporting.DataSources.Sql;
using Blazorise.Reporting.DataSources.WebApi;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
#endregion

namespace Blazorise.Tests.Extensions.Reporting;

public class ReportRendererProviderTest
{
    [Theory]
    [InlineData( false )]
    [InlineData( true )]
    public async Task RenderAsync_Should_Normalize_DataTable_And_DataSet( bool useDataSet )
    {
        DataTable table = CreateTable( "DataSet" );
        object data = table;

        if ( useDataSet )
        {
            DataSet dataSet = new();
            dataSet.Tables.Add( table );
            data = dataSet;
        }

        ReportRenderer renderer = CreateRenderer( new DataSetReportDataSourceProvider() );
        PdfDocumentDefinition document = await renderer.RenderAsync( CreateDefinition( DataSetReportDataSourceProvider.ProviderType ), new()
        {
            DataSources = new Dictionary<string, object>
            {
                ["Data"] = data,
            },
        } );

        AssertRenderedText( document, "DataSet" );
    }

    [Fact]
    public async Task RenderAsync_Should_Load_Csv_Provider()
    {
        CsvReportDataSourceProvider provider = new(
            new FixedHttpClientFactory( new HttpClient() ),
            new CsvReportDataSourceOptions() );
        ReportDefinition definition = CreateDefinition( CsvReportDataSourceProvider.ProviderType, new()
        {
            [CsvReportDataSourceSettings.Source] = "Name\r\nCSV",
        } );

        PdfDocumentDefinition document = await CreateRenderer( provider ).RenderAsync( definition );

        AssertRenderedText( document, "CSV" );
    }

    [Fact]
    public async Task RenderAsync_Should_Load_WebApi_Provider()
    {
        WebApiReportDataSourceOptions options = new();
        RecordingHttpMessageHandler handler = new( new( HttpStatusCode.OK )
        {
            Content = new StringContent( "[{\"Name\":\"Web API\"}]", Encoding.UTF8, "application/json" ),
        } );
        WebApiReportDataSourceProvider provider = new(
            new ServiceCollection().BuildServiceProvider(),
            new FixedHttpClientFactory( new HttpClient( handler ) ),
            options,
            [new JsonReportWebApiResponseReader( options )] );
        ReportDefinition definition = CreateDefinition( WebApiReportDataSourceProvider.ProviderType, new()
        {
            [WebApiReportDataSourceSettings.Url] = "https://8.8.8.8/data",
            [WebApiReportDataSourceSettings.ResponseFormat] = WebApiReportDataSourceFormats.Json,
        } );

        PdfDocumentDefinition document = await CreateRenderer( provider ).RenderAsync( definition );

        AssertRenderedText( document, "Web API" );
        Assert.Equal( 1, handler.RequestCount );
    }

    [Fact]
    public async Task RenderAsync_Should_Load_Sql_Provider()
    {
        DataTable table = CreateTable( "SQL" );
        SqlReportDataSourceOptions options = new()
        {
            QueryAllowed = ( _, _ ) => true,
        };
        options.Connections["Reporting"] = _ => new TestDbConnection( table );
        SqlReportDataSourceProvider provider = new( new ServiceCollection().BuildServiceProvider(), options );
        ReportDefinition definition = CreateDefinition( SqlReportDataSourceProvider.ProviderType, new()
        {
            [SqlReportDataSourceSettings.ConnectionName] = "Reporting",
            [SqlReportDataSourceSettings.Query] = "SELECT Name FROM ReportData",
        } );

        PdfDocumentDefinition document = await CreateRenderer( provider ).RenderAsync( definition );

        AssertRenderedText( document, "SQL" );
    }

    [Fact]
    public async Task RenderAsync_Should_Pass_Parameters_To_Custom_Provider_And_Load_Once()
    {
        RecordingProvider provider = new();
        ReportRenderer renderer = CreateRenderer( provider );

        PdfDocumentDefinition document = await renderer.RenderAsync( CreateDefinition( provider.Type ), new()
        {
            Parameters = new Dictionary<string, object>
            {
                ["TenantId"] = 42,
            },
        } );

        AssertRenderedText( document, "Custom" );
        Assert.Equal( 1, provider.LoadCount );
        Assert.Equal( 42, provider.Parameters["TenantId"] );
    }

    [Fact]
    public async Task RenderAsync_Should_Propagate_Cancellation_To_Provider()
    {
        CancellingProvider provider = new();
        ReportRenderer renderer = CreateRenderer( provider );
        using CancellationTokenSource cancellationTokenSource = new();
        Task<PdfDocumentDefinition> renderTask = renderer.RenderAsync( CreateDefinition( provider.Type ), cancellationToken: cancellationTokenSource.Token );

        await provider.Started.Task;
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>( () => renderTask );
    }

    private static ReportRenderer CreateRenderer( params IReportDataSourceProvider[] providers )
    {
        return new(
            new ReportDataSourceProviderRegistry( providers ),
            new ReportElementPluginRegistry( [] ),
            null );
    }

    private static ReportDefinition CreateDefinition( string providerType, Dictionary<string, object> settings = null )
    {
        return new()
        {
            DataSources =
            [
                new()
                {
                    Name = "Data",
                    ProviderType = providerType,
                    Settings = settings ?? [],
                },
            ],
            Pages =
            [
                new()
                {
                    Bands =
                    [
                        new()
                        {
                            Type = ReportBandType.Detail,
                            DataSource = "Data",
                            Height = 24,
                            Elements =
                            [
                                new ReportFieldElementDefinition
                                {
                                    Field = "Name",
                                    Width = 200,
                                    Height = 18,
                                },
                            ],
                        },
                    ],
                },
            ],
        };
    }

    private static DataTable CreateTable( string value )
    {
        DataTable table = new( "ReportData" );
        table.Columns.Add( "Name", typeof( string ) );
        table.Rows.Add( value );

        return table;
    }

    private static void AssertRenderedText( PdfDocumentDefinition document, string expected )
    {
        Assert.Contains( document.Pages.SelectMany( page => page.Elements ), element => element.Text == expected );
    }

    private sealed class FixedHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient client;

        public FixedHttpClientFactory( HttpClient client )
        {
            this.client = client;
        }

        public HttpClient CreateClient( string name ) => client;
    }

    private sealed class RecordingHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage response;

        public RecordingHttpMessageHandler( HttpResponseMessage response )
        {
            this.response = response;
        }

        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellationToken )
        {
            RequestCount++;
            response.RequestMessage = request;
            return Task.FromResult( response );
        }
    }

    private sealed class RecordingProvider : IReportDataSourceProvider
    {
        public string Type => "custom";

        public string DisplayName => "Custom";

        public Type EditorComponentType => null;

        public int LoadCount { get; private set; }

        public IReadOnlyDictionary<string, object> Parameters { get; private set; }

        public Task<ReportDataSourceSchema> GetSchemaAsync( ReportDataSourceDefinition definition, CancellationToken cancellationToken = default )
            => Task.FromResult<ReportDataSourceSchema>( new() );

        public Task<ReportDataSourceResult> LoadDataAsync( ReportDataSourceDefinition definition, ReportDataSourceLoadContext context, CancellationToken cancellationToken = default )
        {
            LoadCount++;
            Parameters = context.Parameters;
            return Task.FromResult<ReportDataSourceResult>( new()
            {
                Data = new[] { new { Name = "Custom" } },
            } );
        }
    }

    private sealed class CancellingProvider : IReportDataSourceProvider
    {
        public string Type => "cancelling";

        public string DisplayName => "Cancelling";

        public Type EditorComponentType => null;

        public TaskCompletionSource Started { get; } = new( TaskCreationOptions.RunContinuationsAsynchronously );

        public Task<ReportDataSourceSchema> GetSchemaAsync( ReportDataSourceDefinition definition, CancellationToken cancellationToken = default )
            => Task.FromResult<ReportDataSourceSchema>( new() );

        public async Task<ReportDataSourceResult> LoadDataAsync( ReportDataSourceDefinition definition, ReportDataSourceLoadContext context, CancellationToken cancellationToken = default )
        {
            Started.TrySetResult();
            await Task.Delay( Timeout.InfiniteTimeSpan, cancellationToken );
            return null;
        }
    }

    private sealed class TestDbConnection : DbConnection
    {
        private readonly DataTable table;

        private ConnectionState state;

        public TestDbConnection( DataTable table )
        {
            this.table = table;
        }

        public override string ConnectionString { get; set; }

        public override string Database => "Test";

        public override string DataSource => "Test";

        public override string ServerVersion => "1";

        public override ConnectionState State => state;

        public override void ChangeDatabase( string databaseName )
        {
        }

        public override void Close() => state = ConnectionState.Closed;

        public override void Open() => state = ConnectionState.Open;

        public override Task OpenAsync( CancellationToken cancellationToken )
        {
            cancellationToken.ThrowIfCancellationRequested();
            Open();
            return Task.CompletedTask;
        }

        protected override DbTransaction BeginDbTransaction( IsolationLevel isolationLevel ) => throw new NotSupportedException();

        protected override DbCommand CreateDbCommand() => new TestDbCommand( this, table );
    }

    private sealed class TestDbCommand : DbCommand
    {
        private readonly DataTable table;

        private readonly TestDbParameterCollection parameters = new();

        public TestDbCommand( DbConnection connection, DataTable table )
        {
            DbConnection = connection;
            this.table = table;
        }

        public override string CommandText { get; set; }

        public override int CommandTimeout { get; set; }

        public override CommandType CommandType { get; set; }

        public override bool DesignTimeVisible { get; set; }

        public override UpdateRowSource UpdatedRowSource { get; set; }

        protected override DbConnection DbConnection { get; set; }

        protected override DbParameterCollection DbParameterCollection => parameters;

        protected override DbTransaction DbTransaction { get; set; }

        public override void Cancel()
        {
        }

        public override int ExecuteNonQuery() => 0;

        public override object ExecuteScalar() => null;

        public override void Prepare()
        {
        }

        protected override DbParameter CreateDbParameter() => throw new NotSupportedException();

        protected override DbDataReader ExecuteDbDataReader( CommandBehavior behavior ) => table.CreateDataReader();
    }

    private sealed class TestDbParameterCollection : DbParameterCollection
    {
        private readonly List<DbParameter> items = [];

        public override int Count => items.Count;

        public override object SyncRoot => ( (ICollection)items ).SyncRoot;

        public override int Add( object value )
        {
            items.Add( (DbParameter)value );
            return items.Count - 1;
        }

        public override void AddRange( Array values )
        {
            foreach ( object value in values )
                Add( value );
        }

        public override void Clear() => items.Clear();

        public override bool Contains( object value ) => items.Contains( (DbParameter)value );

        public override bool Contains( string value ) => IndexOf( value ) >= 0;

        public override void CopyTo( Array array, int index ) => ( (ICollection)items ).CopyTo( array, index );

        public override IEnumerator GetEnumerator() => items.GetEnumerator();

        public override int IndexOf( object value ) => items.IndexOf( (DbParameter)value );

        public override int IndexOf( string parameterName ) => items.FindIndex( parameter => parameter.ParameterName == parameterName );

        public override void Insert( int index, object value ) => items.Insert( index, (DbParameter)value );

        public override void Remove( object value ) => items.Remove( (DbParameter)value );

        public override void RemoveAt( int index ) => items.RemoveAt( index );

        public override void RemoveAt( string parameterName ) => RemoveAt( IndexOf( parameterName ) );

        protected override DbParameter GetParameter( int index ) => items[index];

        protected override DbParameter GetParameter( string parameterName ) => items[IndexOf( parameterName )];

        protected override void SetParameter( int index, DbParameter value ) => items[index] = value;

        protected override void SetParameter( string parameterName, DbParameter value )
        {
            int index = IndexOf( parameterName );

            if ( index < 0 )
                items.Add( value );
            else
                items[index] = value;
        }
    }
}