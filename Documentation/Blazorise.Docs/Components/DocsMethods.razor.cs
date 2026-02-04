#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Docs.Services;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Docs.Components;

public partial class DocsMethods : IDisposable
{
    #region Methods

    protected override void OnInitialized()
    {
        ThemeService.ThemeChanged += OnThemeChanged;

        base.OnInitialized();
    }

    public void Dispose()
    {
        ThemeService.ThemeChanged -= OnThemeChanged;
    }

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

    void OnThemeChanged( object sender, string theme )
    {
        InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    private ThemeContrast ThemeContrast => ThemeService.ShouldDark ? ThemeContrast.Dark : ThemeContrast.Light;

    [Inject] private ThemeService ThemeService { get; set; }

    [Parameter] public string Title { get; set; }

    [Parameter] public bool Ordered { get; set; } = true;

    [Parameter] public bool ShowReturnType { get; set; } = true;

    [Parameter] public bool ShowParameters { get; set; } = true;

    [Parameter] public RenderFragment ChildContent { get; set; }

    [Parameter] public string Name { get; set; } = "Name";

    public List<DocsMethodsItem> DocsMethodsItems { get; set; } = new();

    #endregion
}