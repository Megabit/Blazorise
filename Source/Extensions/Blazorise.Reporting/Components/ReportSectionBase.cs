using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

public abstract class ReportSectionBase : ComponentBase
{
    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    internal ReportSectionContext SectionContext { get; private set; }

    protected abstract ReportSectionType SectionType { get; }

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
        } );

        SectionContext = new( section );
    }

    [Parameter] public string Name { get; set; }

    [Parameter] public ReportLayout Layout { get; set; } = ReportLayout.Absolute;

    [Parameter] public double Height { get; set; } = 80;

    [Parameter] public string DataSource { get; set; }

    [Parameter] public string Class { get; set; }

    [Parameter] public string Style { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; }
}