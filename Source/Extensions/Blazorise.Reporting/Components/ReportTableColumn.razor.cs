using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

public partial class ReportTableColumn : ComponentBase
{
    [CascadingParameter] internal ReportElementDefinition TableDefinition { get; set; }

    protected override void OnParametersSet()
    {
        if ( TableDefinition is null )
            return;

        TableDefinition.Columns.Add( new()
        {
            Title = Title,
            Field = Field,
            Format = Format,
            Width = Width,
        } );
    }

    [Parameter] public string Title { get; set; }

    [Parameter] public string Field { get; set; }

    [Parameter] public string Format { get; set; }

    [Parameter] public double Width { get; set; } = 120;
}