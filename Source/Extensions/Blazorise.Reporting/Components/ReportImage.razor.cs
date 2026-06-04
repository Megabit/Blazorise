using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

public partial class ReportImage : ReportElementBase
{
    protected override ReportElementType ElementType => ReportElementType.Image;

    protected override ReportElementDefinition BuildDefinition()
    {
        var definition = base.BuildDefinition();
        definition.Source = Source;
        definition.Text = Alt;
        return definition;
    }

    [Parameter] public string Source { get; set; }

    [Parameter] public string Alt { get; set; }
}