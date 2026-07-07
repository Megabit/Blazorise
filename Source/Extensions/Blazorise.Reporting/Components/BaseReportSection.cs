using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

/// <summary>
/// Base class for declarative report bands that register themselves with the current report.
/// </summary>
public abstract class BaseReportSection : ComponentBase
{
    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    /// <summary>
    /// Section context provided to child report elements.
    /// </summary>
    internal ReportSectionContext SectionContext { get; private set; }

    /// <summary>
    /// Band kind represented by the derived component.
    /// </summary>
    protected abstract ReportSectionType SectionType { get; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if ( ReportContext is null )
            return;

        var section = ReportContext.RegisterSection( new()
        {
            Name = Name,
            Type = SectionType,
            Height = Height,
            DataSource = DataSource,
            GroupBy = GroupBy,
            Class = Class,
            Style = Style,
            Default = true,
            Suppress = Suppress ?? false,
            ReserveSpaceWhenSuppressed = ReserveSpaceWhenSuppressed,
            PrintOnFirstPage = PrintOnFirstPage,
            PrintOnLastPage = PrintOnLastPage,
            RepeatOnEveryPage = RepeatOnEveryPage,
            KeepTogether = KeepTogether,
            NewPageBefore = NewPageBefore,
            NewPageAfter = NewPageAfter,
            Appearance = new()
            {
                BackgroundColor = BackgroundColor,
            },
            Border = new()
            {
                Color = BorderColor,
                Width = BorderWidth,
            },
        } );

        SectionContext = new( section );
    }

    /// <summary>
    /// Friendly band name shown in the designer.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Band height in points.
    /// </summary>
    [Parameter] public double Height { get; set; } = 60;

    /// <summary>
    /// Data source name or path used as the band field context. Detail bands repeat when this value resolves to a collection.
    /// </summary>
    [Parameter] public string DataSource { get; set; }

    /// <summary>
    /// Field expression used by group header bands to split detail rows into groups.
    /// </summary>
    [Parameter] public string GroupBy { get; set; }

    /// <summary>
    /// Additional CSS classes applied to the band.
    /// </summary>
    [Parameter] public string Class { get; set; }

    /// <summary>
    /// Inline style applied to the band.
    /// </summary>
    [Parameter] public string Style { get; set; }

    /// <summary>
    /// Excludes the band from rendered output while keeping it visible in the designer.
    /// </summary>
    [Parameter] public ReportValue<bool> Suppress { get; set; }

    /// <summary>
    /// Keeps the band height reserved when the band is suppressed.
    /// </summary>
    [Parameter] public bool ReserveSpaceWhenSuppressed { get; set; }

    /// <summary>
    /// Allows page footer bands to render on the first page.
    /// </summary>
    [Parameter] public bool PrintOnFirstPage { get; set; } = true;

    /// <summary>
    /// Allows page footer bands to render on the last page.
    /// </summary>
    [Parameter] public bool PrintOnLastPage { get; set; } = true;

    /// <summary>
    /// Allows page footer bands to repeat on every rendered page.
    /// </summary>
    [Parameter] public bool RepeatOnEveryPage { get; set; } = true;

    /// <summary>
    /// Keeps the band content together when pagination is applied.
    /// </summary>
    [Parameter] public ReportValue<bool> KeepTogether { get; set; } = false;

    /// <summary>
    /// Starts the band on a new page before rendering it.
    /// </summary>
    [Parameter] public ReportValue<bool> NewPageBefore { get; set; } = false;

    /// <summary>
    /// Starts a new page after the band is rendered.
    /// </summary>
    [Parameter] public ReportValue<bool> NewPageAfter { get; set; } = false;

    /// <summary>
    /// Background color applied to the band.
    /// </summary>
    [Parameter] public ReportColor BackgroundColor { get; set; }

    /// <summary>
    /// Border color applied around the band.
    /// </summary>
    [Parameter] public ReportColor BorderColor { get; set; }

    /// <summary>
    /// Border width applied around the band.
    /// </summary>
    [Parameter] public double? BorderWidth { get; set; }

    /// <summary>
    /// Declarative elements placed inside the band.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
}