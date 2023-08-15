using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Docs.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.Docs.Components.Commercial;

public partial class ThemePage
{
    int checkedProductId;

    protected override Task OnInitializedAsync()
    {
        checkedProductId = Products?.Select( x => x.Id )?.FirstOrDefault() ?? 0;

        return base.OnInitializedAsync();
    }

    async Task OnPurchaseClicked()
    {
        await JSRuntime.InvokeVoidAsync( "blazorisePRO.paddle.openCheckout", checkedProductId );
    }

    bool PurchaseDisabled => checkedProductId == 0;

    [Inject] IJSRuntime JSRuntime { get; set; }

    [Parameter] public string SeoCanonical { get; set; }

    [Parameter] public string SeoTitle { get; set; }

    [Parameter] public string SeoDescription { get; set; }

    [Parameter] public string PreviewUrl { get; set; }

    [Parameter] public string ImageUrl { get; set; }

    [Parameter] public string ImageText { get; set; }

    [Parameter] public RenderFragment Title { get; set; }

    [Parameter] public RenderFragment Description { get; set; }

    [Parameter] public IEnumerable<ThemeInfo> Products { get; set; }
}
