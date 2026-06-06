using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

/// <summary>
/// Base class for declarative report bands that register themselves with the current report.
/// </summary>
public abstract class ReportSectionBase : ComponentBase
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
            Layout = Layout,
            Height = Height,
            DataSource = DataSource,
            Class = Class,
            Style = Style,
            Default = true,
            Suppressed = Suppressed,
        } );

        SectionContext = new( section );
    }

    /// <summary>
    /// Friendly band name shown in the designer.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Layout mode used for elements inside the band.
    /// </summary>
    [Parameter] public ReportLayout Layout { get; set; } = ReportLayout.Absolute;

    /// <summary>
    /// Band height in designer units.
    /// </summary>
    [Parameter] public double Height { get; set; } = 80;

    /// <summary>
    /// Data source name or path used as the band field context. Detail bands repeat when this value resolves to a collection.
    /// </summary>
    [Parameter] public string DataSource { get; set; }

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
    [Parameter] public bool Suppressed { get; set; }

    /// <summary>
    /// Declarative elements placed inside the band.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
}