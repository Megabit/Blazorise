using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Components;

public class RouterTabsItem
{
    public string TypeName { get; set; }
    public string Url { get; set; }
    public RenderFragment Body { get; set; }

}

public class RouterTabsService
{

    internal event Action OnStateHasChanged;

    private List<RouterTabsItem> _tabs = new List<RouterTabsItem>();
    private readonly NavigationManager navigationManager;
    public IReadOnlyCollection<RouterTabsItem> Tabs => _tabs.AsReadOnly();

    public RouterTabsService( NavigationManager navigationManager )
    {
        this.navigationManager = navigationManager;
    }

    internal string CurrentUrl
    {
        get => "/" + navigationManager.ToBaseRelativePath( navigationManager.Uri );
        set
        {
            try
            {
                navigationManager.NavigateTo( value.StartsWith( '/' ) ? value[1..] : value );
            }
            catch ( NavigationException )
            {
                
            }
        }
    }

    private void AddRouterTabsItem( RouterTabsItem routerTabsItem )
    {
        _tabs.Add( routerTabsItem );
    }

    internal void TrySetRouteData( RouteData routeData )
    {
        var routerTabsItem = _tabs?.FirstOrDefault( w => w.Url == CurrentUrl );

        if ( routerTabsItem is null )
        {
            routerTabsItem = new RouterTabsItem
            {
                Url = CurrentUrl,
            };

            AddRouterTabsItem( routerTabsItem );
        }

        if ( routeData is null )
        {

        }
        else
        {
            routerTabsItem.Body ??= CreateRouterTabsItemBody( routeData, routerTabsItem );
            routerTabsItem.TypeName = routeData.PageType.FullName;
        }

        OnStateHasChanged?.Invoke();
    }

    private RenderFragment CreateRouterTabsItemBody( RouteData routeData, RouterTabsItem item )
    {
        return builder =>
        {
            builder.OpenComponent( 0, routeData.PageType );
            foreach ( var routeValue in routeData.RouteValues )
            {
                builder.AddAttribute( 1, routeValue.Key, routeValue.Value );
            }

            builder.AddComponentReferenceCapture( 2, @ref =>
            {
                SetRouterTabsItemFromPage( item, routeData.PageType, item.Url);
            } );

            builder.CloseComponent();
        };
    }

    private void SetRouterTabsItemFromPage( RouterTabsItem pageItem, Type pageType, string url)
    {
        //Get metadata from specialized page attributes
    }
}
