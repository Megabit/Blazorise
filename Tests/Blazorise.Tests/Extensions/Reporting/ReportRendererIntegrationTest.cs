#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Pdf;
using Blazorise.Reporting;
using Blazorise.Reporting.Internal;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Xunit;
#endregion

namespace Blazorise.Tests.Extensions.Reporting;

public class ReportRendererIntegrationTest : BunitContext
{
    private readonly CountingProvider countingProvider = new();

    public ReportRendererIntegrationTest()
    {
        Services.AddBlazoriseTests()
            .AddBootstrapProviders()
            .AddEmptyIconProvider()
            .AddTestData()
            .AddBlazoriseReporting();
        Services.AddSingleton<IReportDataSourceProvider>( countingProvider );
        Services.AddScoped<IModalService, ModalService>();
        JSInterop.AddBlazoriseUtilities();
    }

    [Fact]
    public async Task RenderAsync_Should_Render_Custom_Plugin()
    {
        TestPlugin plugin = new();
        ReportRenderer renderer = new(
            new ReportDataSourceProviderRegistry( [] ),
            new ReportElementPluginRegistry( [plugin] ),
            null );
        ReportDefinition definition = CreateStaticDefinition( new ReportCustomElementDefinition
        {
            TypeName = TestPlugin.TypeName,
            Width = 100,
            Height = 20,
        } );

        PdfDocumentDefinition document = await renderer.RenderAsync( definition );

        Assert.Contains( document.Pages.SelectMany( page => page.Elements ), element => element.Text == "Plugin" );
    }

    [Fact]
    public async Task RenderAsync_Should_Render_Subreport()
    {
        ReportDefinition subreport = CreateStaticDefinition( new ReportTextElementDefinition
        {
            Text = "Subreport",
            Width = 100,
            Height = 18,
        } );
        ReportDefinition definition = CreateStaticDefinition( new ReportSubreportElementDefinition
        {
            Report = subreport,
            Width = 200,
            Height = 100,
        } );
        ReportRenderer renderer = new(
            new ReportDataSourceProviderRegistry( [] ),
            new ReportElementPluginRegistry( [] ),
            null );

        PdfDocumentDefinition document = await renderer.RenderAsync( definition );

        Assert.Contains( document.Pages.SelectMany( page => page.Elements ), element => element.Text == "Subreport" );
    }

    [Fact]
    public void AddBlazoriseReporting_Should_Register_Renderer_As_Scoped()
    {
        ServiceCollection services = new();

        services.AddSingleton<IJSRuntime>( JSInterop.JSRuntime );
        services.AddBlazorise().AddBlazoriseReporting();

        ServiceDescriptor descriptor = Assert.Single( services, descriptor => descriptor.ServiceType == typeof( IReportRenderer ) );
        Assert.Equal( ServiceLifetime.Scoped, descriptor.Lifetime );

        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        using IServiceScope scope = serviceProvider.CreateScope();
        Assert.IsType<ReportRenderer>( scope.ServiceProvider.GetRequiredService<IReportRenderer>() );
    }

    [Fact]
    public async Task Mounted_Report_And_Backend_Renderer_Should_Create_Equivalent_Pdf_Definitions()
    {
        ReportDefinition definition = CreateDataDefinition();
        object data = new[] { new DataItem( "Parity" ) };
        IRenderedComponent<Report> component = Render<Report>( parameters => parameters
            .Add( report => report.Definition, definition )
            .Add( report => report.DefinitionMode, ReportDefinitionMode.UseDefinitionOnly )
            .Add( report => report.PreviewFormats, ReportPreviewFormat.None )
            .Add( report => report.Data, data ) );

        PdfDocumentDefinition componentDocument = await component.Instance.GetPdfDocument();
        IReportRenderer renderer = Services.GetRequiredService<IReportRenderer>();
        PdfDocumentDefinition backendDocument = await renderer.RenderAsync( definition, new()
        {
            DefaultData = data,
        } );

        Assert.Equal( GetElementSignatures( backendDocument ), GetElementSignatures( componentDocument ) );
    }

    [Fact]
    public void Pdf_Preview_Should_Load_Provider_Once()
    {
        ReportDefinition definition = CreateDataDefinition();
        definition.DataSources[0].ProviderType = countingProvider.Type;

        IRenderedComponent<Report> component = Render<Report>( parameters => parameters
            .Add( report => report.Definition, definition )
            .Add( report => report.DefinitionMode, ReportDefinitionMode.UseDefinitionOnly )
            .Add( report => report.Mode, ReportMode.Preview )
            .Add( report => report.PreviewFormat, ReportPreviewFormat.Pdf )
            .Add( report => report.PreviewFormats, ReportPreviewFormat.Pdf ) );
        IRenderedComponent<_ReportDesigner> designer = component.FindComponent<_ReportDesigner>();

        designer.WaitForAssertion( () =>
        {
            ReportPdfPreviewContext preview = designer.Instance.PdfPreviewContext;

            Assert.NotNull( preview );
            Assert.NotEmpty( preview.Content );
            Assert.Equal( "application/pdf", preview.ContentType );
            Assert.Equal( 1, countingProvider.LoadCount );
        }, TestExtensions.WaitTime );
    }

    private static ReportDefinition CreateStaticDefinition( ReportElementDefinition element )
    {
        return new()
        {
            Pages =
            [
                new()
                {
                    Bands =
                    [
                        new()
                        {
                            Type = ReportBandType.ReportHeader,
                            Height = 120,
                            Elements = [element],
                        },
                    ],
                },
            ],
        };
    }

    private static ReportDefinition CreateDataDefinition()
    {
        ReportDefinition definition = CreateStaticDefinition( new ReportFieldElementDefinition
        {
            Field = nameof( DataItem.Name ),
            Width = 100,
            Height = 18,
        } );
        definition.Pages[0].Bands[0].Type = ReportBandType.Detail;
        definition.Pages[0].Bands[0].DataSource = "Data";
        definition.DataSources =
        [
            new()
            {
                Name = "Data",
                ProviderType = ObjectReportDataSourceProvider.ProviderType,
            },
        ];

        return definition;
    }

    private static IReadOnlyList<string> GetElementSignatures( PdfDocumentDefinition document )
    {
        return document.Pages
            .SelectMany( page => page.Elements )
            .Select( element => $"{element.Type}|{element.Text}|{element.X}|{element.Y}|{element.Width}|{element.Height}" )
            .ToList();
    }

    private sealed record DataItem( string Name );

    private sealed class TestPlugin : IReportElementPlugin, IReportElementPdfRenderer
    {
        public const string TypeName = "tests.plugin";

        public ReportElementDescriptor Descriptor { get; } = new()
        {
            TypeName = TypeName,
            DisplayName = "Test plugin",
            Width = 100,
            Height = 20,
        };

        public Type RendererComponentType => typeof( TestPluginComponent );

        public Type PropertiesComponentType => null;

        public IReportElementPdfRenderer PdfRenderer => this;

        public ReportCustomElementDefinition CreateElement() => new();

        public IEnumerable<PdfElementDefinition> Render( ReportElementPdfRenderContext context )
        {
            yield return new()
            {
                Type = PdfElementType.Text,
                Text = "Plugin",
                Width = context.Element.Width,
                Height = context.Element.Height,
            };
        }
    }

    private sealed class TestPluginComponent : BaseReportElementRenderer
    {
    }

    private sealed class CountingProvider : IReportDataSourceProvider
    {
        public string Type => "counting";

        public string DisplayName => "Counting";

        public Type EditorComponentType => null;

        public int LoadCount { get; private set; }

        public Task<ReportDataSourceSchema> GetSchemaAsync( ReportDataSourceDefinition definition, CancellationToken cancellationToken = default )
            => Task.FromResult<ReportDataSourceSchema>( new() );

        public Task<ReportDataSourceResult> LoadDataAsync( ReportDataSourceDefinition definition, ReportDataSourceLoadContext context, CancellationToken cancellationToken = default )
        {
            LoadCount++;
            return Task.FromResult<ReportDataSourceResult>( new()
            {
                Data = new[] { new DataItem( "Single load" ) },
            } );
        }
    }
}