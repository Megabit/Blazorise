#region Using directives
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Reporting;
using Blazorise.Reporting.DataSources.Sql;
using Blazorise.Reporting.DataSources.WebApi;
using Blazorise.Reporting.Internal;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
#endregion

namespace Blazorise.Tests.Extensions.Reporting;

public class ReportingDataSourceSecurityTest
{
    [Theory]
    [InlineData( false )]
    [InlineData( true )]
    public async Task ConnectionDialog_Should_Block_Closing_While_Connecting( bool isConnecting )
    {
        ReportDataSourceConnectionDialogSession session = new()
        {
            IsConnecting = isConnecting,
        };
        ModalClosingEventArgs eventArgs = new( false, CloseReason.EscapeClosing );

        await session.OnClosing( eventArgs );

        Assert.Equal( isConnecting, eventArgs.Cancel );
    }

    [Fact]
    public void ProviderEditorContext_Should_Notify_Only_When_A_Setting_Changes()
    {
        int notifications = 0;
        ReportDataSourceProviderEditorContext context = new( "test", settingsChanged: () => notifications++ );

        context.SetValue( "Url", "https://example.com" );
        context.SetValue( "Url", "https://example.com" );
        context.SetValue( "Url", null );

        Assert.Equal( 2, notifications );
    }

    [Fact]
    public void ProviderEditorContext_Should_Resolve_Disabled_State()
    {
        bool disabled = false;
        ReportDataSourceProviderEditorContext context = new( "test" )
        {
            ResolveDisabled = () => disabled,
        };

        Assert.False( context.Disabled );

        disabled = true;

        Assert.True( context.Disabled );
    }

    [Fact]
    public async Task Failed_Connection_Preparation_Should_Not_Mutate_The_Report()
    {
        ReportDataSourceDefinition existingDataSource = new()
        {
            Id = "source-1",
            Name = "Orders",
            ProviderType = "test",
            Settings = new()
            {
                ["Value"] = "original",
            },
        };
        ReportDefinition definition = new()
        {
            DataSources = [existingDataSource],
        };
        ReportDataSourceDefinition candidate = new()
        {
            Id = existingDataSource.Id,
            Name = existingDataSource.Name,
            ProviderType = existingDataSource.ProviderType,
            Settings = new()
            {
                ["Value"] = "changed",
            },
        };
        ReportDataCommandService service = new();

        ReportDefinition result = await service.PrepareDataSourceConnection(
            definition,
            null,
            candidate,
            ( _, _ ) => Task.FromResult( false ) );

        Assert.Null( result );
        Assert.Equal( "original", definition.DataSources[0].Settings["Value"] );
    }

    [Fact]
    public async Task Sql_Provider_Should_Deny_Queries_Before_Creating_A_Connection()
    {
        bool connectionCreated = false;
        SqlReportDataSourceOptions options = new();
        options.Connections["Reporting"] = _ =>
        {
            connectionCreated = true;
            return null;
        };
        ServiceProvider serviceProvider = new ServiceCollection().BuildServiceProvider();
        SqlReportDataSourceProvider provider = new( serviceProvider, options );
        ReportDataSourceDefinition definition = new()
        {
            Settings = new()
            {
                [SqlReportDataSourceSettings.ConnectionName] = "Reporting",
                [SqlReportDataSourceSettings.Query] = "SELECT 1",
            },
        };

        await Assert.ThrowsAsync<InvalidOperationException>( () => provider.GetSchemaAsync( definition ) );

        Assert.False( connectionCreated );
    }

    [Theory]
    [InlineData( "utf-8" )]
    [InlineData( "utf-16" )]
    [InlineData( "utf-16BE" )]
    [InlineData( "utf-32" )]
    [InlineData( "utf-32BE" )]
    public async Task Xml_Reader_Should_Detect_Bom_Prefixed_Content( string encodingName )
    {
        Encoding encoding = encodingName switch
        {
            "utf-16" => Encoding.Unicode,
            "utf-16BE" => Encoding.BigEndianUnicode,
            "utf-32" => Encoding.UTF32,
            "utf-32BE" => new UTF32Encoding( bigEndian: true, byteOrderMark: true ),
            _ => new UTF8Encoding( encoderShouldEmitUTF8Identifier: true ),
        };
        byte[] content = [.. encoding.GetPreamble(), .. encoding.GetBytes( " \r\n<orders><order /></orders>" )];
        XmlReportWebApiResponseReader reader = new( new() );

        Assert.True( reader.CanRead( null, content ) );

        ReportDataSourceResult result = await reader.ReadAsync( content, "/orders/order" );
        Assert.NotNull( result.Schema );
    }

    [Fact]
    public async Task Json_Reader_Should_Enforce_Collection_Limits()
    {
        JsonReportWebApiResponseReader reader = new( new()
        {
            MaximumCollectionItems = 1,
        } );
        byte[] content = Encoding.UTF8.GetBytes( "[{},{}]" );

        await Assert.ThrowsAsync<InvalidOperationException>( () => reader.ReadAsync( content, null ) );
    }

    [Theory]
    [InlineData( "http://localhost/data" )]
    [InlineData( "http://127.0.0.1/data" )]
    [InlineData( "http://[::ffff:127.0.0.1]/data" )]
    public async Task WebApi_Provider_Should_Reject_NonPublic_Destinations( string url )
    {
        RecordingHttpMessageHandler handler = new( _ => new( HttpStatusCode.OK )
        {
            Content = new StringContent( "{}", Encoding.UTF8, "application/json" ),
        } );
        WebApiReportDataSourceProvider provider = CreateWebApiProvider( handler );

        await Assert.ThrowsAsync<InvalidOperationException>( () => provider.GetSchemaAsync( CreateWebApiDefinition( url ) ) );

        Assert.Equal( 0, handler.RequestCount );
    }

    [Fact]
    public async Task WebApi_Provider_Should_Reject_Restricted_Report_Headers()
    {
        RecordingHttpMessageHandler handler = new( _ => new( HttpStatusCode.OK ) );
        WebApiReportDataSourceProvider provider = CreateWebApiProvider( handler );
        ReportDataSourceDefinition definition = CreateWebApiDefinition( "https://8.8.8.8/data" );
        definition.Settings[WebApiReportDataSourceSettings.Headers] = "Host: localhost";

        await Assert.ThrowsAsync<InvalidOperationException>( () => provider.GetSchemaAsync( definition ) );

        Assert.Equal( 0, handler.RequestCount );
    }

    [Fact]
    public async Task WebApi_Provider_Should_Reject_Oversized_Responses()
    {
        RecordingHttpMessageHandler handler = new( _ => new( HttpStatusCode.OK )
        {
            Content = new ByteArrayContent( Encoding.UTF8.GetBytes( "{\"value\":1}" ) ),
        } );
        WebApiReportDataSourceProvider provider = CreateWebApiProvider( handler, maximumResponseSize: 4 );

        await Assert.ThrowsAsync<System.IO.InvalidDataException>( () => provider.GetSchemaAsync( CreateWebApiDefinition( "https://8.8.8.8/data" ) ) );

        Assert.Equal( 1, handler.RequestCount );
    }

    [Fact]
    public async Task WebApi_Provider_Should_Reject_Redirects()
    {
        RecordingHttpMessageHandler handler = new( _ => new( HttpStatusCode.Redirect ) );
        WebApiReportDataSourceProvider provider = CreateWebApiProvider( handler );

        await Assert.ThrowsAsync<InvalidOperationException>( () => provider.GetSchemaAsync( CreateWebApiDefinition( "https://8.8.8.8/data" ) ) );

        Assert.Equal( 1, handler.RequestCount );
    }

    private static WebApiReportDataSourceProvider CreateWebApiProvider( HttpMessageHandler handler, long maximumResponseSize = 1024 )
    {
        WebApiReportDataSourceOptions options = new()
        {
            MaximumResponseSize = maximumResponseSize,
        };
        ServiceProvider serviceProvider = new ServiceCollection().BuildServiceProvider();
        FixedHttpClientFactory httpClientFactory = new( new( handler ) );
        IReportWebApiResponseReader[] readers =
        [
            new JsonReportWebApiResponseReader( options ),
            new XmlReportWebApiResponseReader( options ),
        ];

        return new( serviceProvider, httpClientFactory, options, readers );
    }

    private static ReportDataSourceDefinition CreateWebApiDefinition( string url )
    {
        return new()
        {
            Settings = new()
            {
                [WebApiReportDataSourceSettings.Url] = url,
                [WebApiReportDataSourceSettings.ResponseFormat] = WebApiReportDataSourceFormats.Json,
            },
        };
    }

    private sealed class FixedHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient httpClient;

        internal FixedHttpClientFactory( HttpClient httpClient )
        {
            this.httpClient = httpClient;
        }

        public HttpClient CreateClient( string name )
        {
            return httpClient;
        }
    }

    private sealed class RecordingHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> responseFactory;

        internal RecordingHttpMessageHandler( Func<HttpRequestMessage, HttpResponseMessage> responseFactory )
        {
            this.responseFactory = responseFactory;
        }

        internal int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellationToken )
        {
            RequestCount++;
            HttpResponseMessage response = responseFactory( request );
            response.RequestMessage ??= request;

            return Task.FromResult( response );
        }
    }
}