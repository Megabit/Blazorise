using System;
using Blazorise.Docs.Services;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Docs.Layouts;

public class BaseDocsLayout : LayoutComponentBase, IDisposable
{
    private bool disposed;

    [Inject] protected ThemeService ThemeService { get; set; }

    protected override void OnInitialized()
    {
        ThemeService.ThemeChanged += OnThemeChanged;

        base.OnInitialized();
    }

    protected virtual void Dispose( bool disposing )
    {
        if ( !disposed )
        {
            if ( disposing )
            {
                ThemeService.ThemeChanged -= OnThemeChanged;
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose( true );
        GC.SuppressFinalize( this );
    }

    void OnThemeChanged( object sender, string theme )
    {
        InvokeAsync( StateHasChanged );
    }
}
