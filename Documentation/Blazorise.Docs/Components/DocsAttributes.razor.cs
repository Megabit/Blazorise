#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Docs.Services;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Docs.Components;

public partial class DocsAttributes : IDisposable
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
            DocsAttributesItems = DocsAttributesItems.OrderBy( x => x.Name ).ToList();
            return InvokeAsync( StateHasChanged );
        }
        return Task.CompletedTask;
    }

    internal void AddItem( DocsAttributesItem item )
        => DocsAttributesItems.Add( item );

    internal bool RemoveItem( DocsAttributesItem item )
        => DocsAttributesItems.Remove( item );

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

    [Parameter] public bool ShowDefaults { get; set; } = true;

    [Parameter] public RenderFragment ChildContent { get; set; }

    public List<DocsAttributesItem> DocsAttributesItems { get; set; } = new();

    #endregion
}