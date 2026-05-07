#region Using directives
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Docs.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Docs.Components;

public partial class DocsPageSectionSource
{
    #region Methods

    protected override void OnInitialized()
    {
        CurrentCode = Code;
    }

    private Task OnToggleCode()
    {
        ShowCode = !ShowCode;

        return Task.CompletedTask;
    }

    private async Task OnCopyCode()
    {
        await JSRuntime.InvokeVoidAsync( "blazoriseDocs.code.copyToClipboard", Snippets.GetCode( Code ) );
        await NotificationService.Info( $"Copied code example!" );
    }

    private RenderFragment CodeComponent( string code ) => builder =>
    {
        try
        {
            var key = typeof( DocsPageSectionSource ).Assembly.GetManifestResourceNames().FirstOrDefault( x => x.Contains( $".{code}Code.html" ) );

            if ( key != null )
            {
                using var stream = typeof( DocsPageSectionSource ).Assembly.GetManifestResourceStream( key );
                using var reader = new StreamReader( stream );
                builder.AddMarkupContent( 0, reader.ReadToEnd() );
            }
        }
        catch ( Exception )
        {
        }
    };

    #endregion

    #region Properties

    private IFluentDisplay SourceCodeDisplay => ShowCode ? Display.Block : Display.None;

    private string CurrentCode { get; set; }

    [Inject] public INotificationService NotificationService { get; set; }

    [Inject] public IJSRuntime JSRuntime { get; set; }

    [Parameter] public string Code { get; set; }

    [Parameter] public bool ShowCode { get; set; } = true;

    #endregion
}