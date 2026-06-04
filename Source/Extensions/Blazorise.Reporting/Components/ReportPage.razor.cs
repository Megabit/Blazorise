using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

public partial class ReportPage : ComponentBase
{
    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    protected override void OnParametersSet()
    {
        if ( ReportContext is not null )
        {
            ReportContext.RegisterPage( Page );
        }
    }

    [Parameter] public ReportPageSize Size { get; set; } = ReportPageSize.A4;

    [Parameter] public ReportOrientation Orientation { get; set; } = ReportOrientation.Portrait;

    [Parameter] public double? Width { get; set; }

    [Parameter] public double? Height { get; set; }

    internal ReportPageDefinition Page => new()
    {
        Size = Size,
        Orientation = Orientation,
        Width = Width ?? 0,
        Height = Height ?? 0,
    };
}