#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Components;

internal class RouterTabsItem
{
    public string Name { get; set; }
    public string TypeName { get; set; }
    public string Url { get; set; }
    public RenderFragment Body { get; set; }
    public string TabClass { get; set; }
    public string TabPanelClass { get; set; }
    public bool Closeable { get; set; } = true;

}

public class RouterTabsService
{
    #region Properties

    internal event Action StateHasChanged;

    private string selectedRouterTab;
    private List<RouterTabsItem> tabs = new List<RouterTabsItem>();
    private readonly NavigationManager navigationManager;
    internal IReadOnlyCollection<RouterTabsItem> Tabs => tabs.AsReadOnly();

    internal string SelectedRouterTab
    {
        get => selectedRouterTab;
        set
        {
            selectedRouterTab = value;
            var pageItem = tabs.FirstOrDefault( r => r.Name == value );
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

    #endregion

    #region Methods

    internal void AddRouterTab( RouterTabsItem routerTabsItem )
    {
        tabs.Add( routerTabsItem );
    }

    internal void CloseRouterTab( RouterTabsItem routerTabsItem )
    {
        string nextSelected = null;
        if ( selectedRouterTab == routerTabsItem.Name && tabs.Count > 1 )
            for ( int i = 0; i < tabs.Count; i++ )
            {
                if ( i > 0 && tabs[i].Name == selectedRouterTab )
                    nextSelected = tabs[i - 1].Name;
                if ( i > 0 && tabs[i - 1].Name == selectedRouterTab )
                    nextSelected = tabs[i].Name;
            }

        tabs.Remove( routerTabsItem );
        SelectedRouterTab = nextSelected;
        StateHasChanged?.Invoke();
    }

    internal void TrySetRouteData( RouteData routeData )
    {
        var routerTabsItem = tabs?.FirstOrDefault( w => w.Url == CurrentUrl );

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
            if ( string.IsNullOrWhiteSpace( routerTabsItem.Name ) )
                routerTabsItem.Name = routeData.PageType.FullName;
        }

        SelectedRouterTab = routerTabsItem.Name;
        StateHasChanged?.Invoke();
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

    private void SetRouterTabsItemFromPageAttribute( RouterTabsItem pageItem, Type pageType )
    {
        var routerTabsPageAttr = pageType.GetCustomAttribute<RouterTabsPageAttribute>();
        if ( routerTabsPageAttr is not null )
        {
            pageItem.Name = routerTabsPageAttr.Name;
            pageItem.TabClass = routerTabsPageAttr.TabClass;
            pageItem.TabPanelClass = routerTabsPageAttr.TabPanelClass;
            pageItem.Closeable = routerTabsPageAttr.Closeable;
        }
    }

    #endregion
}
