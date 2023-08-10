#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Docs.Components.Commercial;

public partial class PurchaseButton
{
    #region Members

    #endregion

    #region Methods

    async Task OnPurchaseClicked()
    {
        await JSRuntime.InvokeVoidAsync( "blazorisePRO.paddle.openCheckout", ProductId, Quantity, Upsell );
    }

    #endregion

    #region Properties

    [Inject] IJSRuntime JSRuntime { get; set; }

    [Parameter] public Color Color { get; set; }

    [Parameter] public int ProductId { get; set; }

    [Parameter] public int Quantity { get; set; }

    [Parameter] public object Upsell { get; set; }

    [Parameter] public string ProductName { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
