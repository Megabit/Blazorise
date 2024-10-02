#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Docs.Components;

public partial class DocsAttributes
{
    #region Methods

    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        base.OnAfterRenderAsync( firstRender );
        if ( Ordered && firstRender )
        {
            DocsAttributesItems = DocsAttributesItems.OrderBy( x => x.Name ).ToList();
            return InvokeAsync( StateHasChanged );
        }
        return Task.CompletedTask;
    }

    internal void AddItem( DocsAttributesItem item )
        => DocsAttributesItems.Add( item );

    internal bool RemoveItem( DocsAttributesItem item )
        => DocsAttributesItems.Remove( item );

    #endregion

    #region Properties

    [Parameter] public string Title { get; set; }

    [Parameter] public bool Ordered { get; set; } = true;

    [Parameter] public bool ShowDefaults { get; set; } = true;

    [Parameter] public RenderFragment ChildContent { get; set; }

    public List<DocsAttributesItem> DocsAttributesItems { get; set; } = new();

    #endregion
}