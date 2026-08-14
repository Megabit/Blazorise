#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Pdf;
using Blazorise.Reporting;
using Xunit;
#endregion

namespace Blazorise.Tests.Extensions.Reporting;

public class ReportRendererTest
{
    [Fact]
    public async Task RenderAsync_Should_Attach_Named_Data_Without_Mutating_Definition()
    {
        ReportDefinition definition = CreateDefinition();
        ReportRenderer renderer = CreateRenderer();

        PdfDocumentDefinition document = await renderer.RenderAsync( definition, new()
        {
            DataSources = new Dictionary<string, object>
            {
                ["orders"] = new[]
                {
                    new Order( "Order 1" ),
                },
            },
        } );

        Assert.Null( definition.DataSources[0].Data );
        Assert.Single( document.Pages );
        Assert.Contains( document.Pages[0].Elements, element => element.Text == "Order 1" );
    }

    [Fact]
    public async Task RenderAsync_Should_Reject_Missing_Named_Data()
    {
        ReportDefinition definition = CreateDefinition();
        ReportRenderer renderer = CreateRenderer();

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>( () => renderer.RenderAsync( definition ) );

        Assert.Contains( "Orders", exception.Message );
    }

    private static ReportRenderer CreateRenderer()
    {
        IReportDataSourceProvider[] providers =
        [
            new ObjectReportDataSourceProvider(),
            new DataSetReportDataSourceProvider(),
        ];

        return new(
            new ReportDataSourceProviderRegistry( providers ),
            new ReportElementPluginRegistry( [] ),
            null );
    }

    private static ReportDefinition CreateDefinition()
    {
        return new()
        {
            DataSources =
            [
                new()
                {
                    Name = "Orders",
                    ProviderType = ObjectReportDataSourceProvider.ProviderType,
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
                            DataSource = "Orders",
                            Height = 24,
                            Elements =
                            [
                                new ReportFieldElementDefinition
                                {
                                    Field = nameof( Order.Name ),
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

    private sealed record Order( string Name );
}