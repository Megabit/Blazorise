using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

public partial class ReportText : ReportElementBase
{
    protected override ReportElementType ElementType => ReportElementType.Text;

    protected override ReportElementDefinition BuildDefinition()
    {
        var definition = base.BuildDefinition();
        definition.Text = Text;
        return definition;
    }

    [Parameter] public string Text { get; set; }
}