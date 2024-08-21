#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Docs.Components;

public partial class DocsMethods
{
    #region Methods

    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        base.OnAfterRenderAsync( firstRender );
        if ( Ordered && firstRender )
        {
            DocsMethodsItems = DocsMethodsItems.OrderBy( x => x.Name ).ToList();
            return InvokeAsync( StateHasChanged );
        }
        return Task.CompletedTask;
    }

    internal void AddItem( DocsMethodsItem item )
        => DocsMethodsItems.Add( item );

    internal bool RemoveItem( DocsMethodsItem item )
        => DocsMethodsItems.Remove( item );

    #endregion

    #region Properties

    [Parameter] public string Title { get; set; }

    [Parameter] public bool Ordered { get; set; } = true;

    [Parameter] public bool ShowReturnType { get; set; } = true;

    [Parameter] public bool ShowParameters { get; set; } = true;

    [Parameter] public RenderFragment ChildContent { get; set; }

    public List<DocsMethodsItem> DocsMethodsItems { get; set; } = new();

    #endregion
}