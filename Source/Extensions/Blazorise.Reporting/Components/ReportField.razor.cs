using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

public partial class ReportField : ReportElementBase
{
    protected override ReportElementType ElementType => ReportElementType.Field;

    protected override ReportElementDefinition BuildDefinition()
    {
        var definition = base.BuildDefinition();
        definition.Field = Field;
        definition.Format = Format;
        return definition;
    }

    [Parameter] public string Field { get; set; }

    [Parameter] public string Format { get; set; }
}