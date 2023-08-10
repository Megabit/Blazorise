using Microsoft.AspNetCore.Components;

namespace Blazorise.Docs.Components.Commercial;

public partial class ThemePrice
{
    [Parameter] public string Name { get; set; }

    [Parameter] public string Description { get; set; }

    [Parameter] public decimal? Price { get; set; }

    [Parameter] public string Currency { get; set; }
}
