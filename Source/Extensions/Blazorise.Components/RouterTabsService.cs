using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Components;

internal class RouterTabsItem
{
    public string Name { get; set; }
    public string TypeName { get; set; }
    public string Url { get; set; }
    public RenderFragment Body { get; set; }
    public string TabCssClass { get; set; }
    public string TabPanelCssClass { get; set; }
    public bool Closeable { get; set; } = true;

}

public class RouterTabsService
{

    internal event Action OnStateHasChanged;

    private string _selectedRouterTab;
    private List<RouterTabsItem> _tabs = new List<RouterTabsItem>();
    private readonly NavigationManager navigationManager;
    internal IReadOnlyCollection<RouterTabsItem> Tabs => _tabs.AsReadOnly();

    internal string SelectedRouterTab
    {
        get => _selectedRouterTab;
        set
        {
            _selectedRouterTab = value;
            var pageItem = _tabs.FirstOrDefault( r => r.Name == value );
            if ( pageItem != null && ( pageItem.Url != CurrentUrl ) )
            {
                CurrentUrl = pageItem.Url;
            }
        }
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

    public RouterTabsService( NavigationManager navigationManager )
    {
        this.navigationManager = navigationManager;
    }

    internal void AddRouterTab( RouterTabsItem routerTabsItem )
    {
        _tabs.Add( routerTabsItem );
    }

    internal void CloseRouterTab( RouterTabsItem routerTabsItem )
    {
        string nextSelected = null;
        if ( _selectedRouterTab == routerTabsItem.Name && _tabs.Count > 1 )
            for ( int i = 0; i < _tabs.Count; i++ )
            {
                if ( i > 0 && _tabs[i].Name == _selectedRouterTab )
                    nextSelected = _tabs[i - 1].Name;
                if ( i > 0 && _tabs[i - 1].Name == _selectedRouterTab )
                    nextSelected = _tabs[i].Name;
            }

        _tabs.Remove( routerTabsItem );
        SelectedRouterTab = nextSelected;
        OnStateHasChanged?.Invoke();
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

            AddRouterTab( routerTabsItem );
            SetRouterTabsItemFromPageAttribute( routerTabsItem, routeData.PageType );
        }

        if ( routeData is not null )
        {
            routerTabsItem.Body ??= CreateRouterTabsItemBody( routeData );
            routerTabsItem.TypeName = routeData.PageType.FullName;
            if ( string.IsNullOrWhiteSpace(routerTabsItem.Name))
                routerTabsItem.Name = routeData.PageType.FullName;
        }

        SelectedRouterTab = routerTabsItem.Name;
        OnStateHasChanged?.Invoke();
    }

    private RenderFragment CreateRouterTabsItemBody( RouteData routeData )
    {
        return builder =>
        {
            builder.OpenComponent( 0, routeData.PageType );
            foreach ( var routeValue in routeData.RouteValues )
            {
                builder.AddAttribute( 1, routeValue.Key, routeValue.Value );
            }

            builder.CloseComponent();
        };
    }

    private void SetRouterTabsItemFromPageAttribute( RouterTabsItem pageItem, Type pageType)
    {
        var routerTabsPageAttr = pageType.GetCustomAttribute<RouterTabsPageAttribute>();
        if ( routerTabsPageAttr is not null)
        {
            pageItem.Name = routerTabsPageAttr.Title;
            pageItem.TabCssClass = routerTabsPageAttr.TabCssClass;
            pageItem.TabPanelCssClass = routerTabsPageAttr.TabPanelCssClass;
            pageItem.Closeable = routerTabsPageAttr.Closeable;
        }
    }
}
