using System;
using Blazorise.Docs.Services;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Docs.Layouts;

public class BaseDocsLayout : LayoutComponentBase, IDisposable
{
    [Inject] protected ThemeService ThemeService { get; set; }

    protected override void OnInitialized()
    {
        ThemeService.ThemeChanged += OnThemeChanged;

        base.OnInitialized();
    }

    public void Dispose()
    {
        ThemeService.ThemeChanged -= OnThemeChanged;
    }

    void OnThemeChanged( object sender, string theme )
    {
        InvokeAsync( StateHasChanged );
    }
}
