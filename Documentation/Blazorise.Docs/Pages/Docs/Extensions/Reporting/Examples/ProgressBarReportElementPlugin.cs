#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
using Blazorise.Pdf;
using Blazorise.Reporting;
#endregion

namespace Blazorise.Docs.Docs.Examples;

public sealed class ProgressBarReportElementPlugin : IReportElementPlugin, IReportElementPdfRenderer
{
    public const string TypeName = "docs.progress-bar";

    public ReportElementDescriptor Descriptor { get; } = new()
    {
        TypeName = TypeName,
        DisplayName = "Progress Bar",
        Category = "Custom",
        Icon = IconName.ChartBar,
        Width = 200,
        Height = 38,
        Capabilities = ReportElementCapabilities.Default,
    };

    public Type RendererComponentType => typeof( ProgressBarReportElementRenderer );

    public Type PropertiesComponentType => typeof( ProgressBarReportElementProperties );

    public IReportElementPdfRenderer PdfRenderer => this;

    public ReportCustomElementDefinition CreateElement()
    {
        return new()
        {
            Properties = new()
            {
                ["caption"] = "Progress",
                ["value"] = 60,
                ["color"] = "#0D6EFD",
            },
        };
    }

    public IEnumerable<PdfElementDefinition> Render( ReportElementPdfRenderContext context )
    {
        int value = GetValue( context.Element );
        double trackHeight = Math.Min( 12, context.Element.Height );
        double trackY = Math.Max( 0, context.Element.Height - trackHeight );

        yield return CreateLabel( context.Element, GetCaption( context.Element ), TextAlignment.Start );
        yield return CreateLabel( context.Element, $"{value}%", TextAlignment.End );
        yield return CreateRectangle( context.Element.Width, trackY, trackHeight, "#E9ECEF" );
        yield return CreateRectangle( context.Element.Width * value / 100d, trackY, trackHeight, GetColor( context.Element ) );
    }

    internal static string GetCaption( ReportCustomElementDefinition element )
        => element?.Properties?["caption"]?.GetValue<string>() ?? "Progress";

    internal static int GetValue( ReportCustomElementDefinition element )
        => Math.Clamp( element?.Properties?["value"]?.GetValue<int>() ?? 0, 0, 100 );

    internal static string GetColor( ReportCustomElementDefinition element )
        => element?.Properties?["color"]?.GetValue<string>() ?? "#0D6EFD";

    private static PdfElementDefinition CreateLabel( ReportCustomElementDefinition element, string text, TextAlignment alignment )
    {
        return new()
        {
            Type = PdfElementType.Text,
            Width = element.Width,
            Height = Math.Min( 14, element.Height ),
            Text = text,
            Wrap = false,
            Font = new()
            {
                Size = 9,
                Bold = true,
                Alignment = alignment,
                VerticalAlignment = VerticalAlignment.Middle,
                Color = "#212529",
            },
            Border = new()
            {
                Width = 0,
            },
        };
    }

    private static PdfElementDefinition CreateRectangle( double width, double y, double height, string color )
    {
        return new()
        {
            Type = PdfElementType.Rectangle,
            Y = y,
            Width = width,
            Height = height,
            Border = new()
            {
                Width = 0,
            },
            Appearance = new()
            {
                BackgroundColor = color,
            },
        };
    }
}