#region Using directives
using System;
using System.Collections.Generic;
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

    protected override void OnParametersSet()
    {
        if ( string.IsNullOrWhiteSpace( CurrentCode ) || !CodeSources.Any( source => source.Code == CurrentCode ) )
        {
            CurrentCode = Code;
        }
    }

    private Task OnToggleCode()
    {
        ShowCode = !ShowCode;

        return Task.CompletedTask;
    }

    private Task OnToggleSourceExpanded()
    {
        SourceExpanded = !SourceExpanded;

        return Task.CompletedTask;
    }

    private async Task OnCopyCode()
    {
        await JSRuntime.InvokeVoidAsync( "blazoriseDocs.code.copyToClipboard", Snippets.GetCode( CurrentCode ) );
        await NotificationService.Info( $"Copied code example!" );
    }

    private Task OnSelectCodeSource( string code )
    {
        CurrentCode = code;

        return Task.CompletedTask;
    }

    private TextColor SourceSelectorTextColor( string code )
        => CurrentCode == code ? TextColor.Primary : TextColor.Body;

    private TextWeight SourceSelectorTextWeight( string code )
        => CurrentCode == code ? TextWeight.SemiBold : TextWeight.Normal;

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

    private IFluentSizing SourceCodeHeight => SourceExpanded ? null : Height.Px().Max( 280 );

    private bool SourceExpanded { get; set; }

    private IReadOnlyList<DocsCodeSource> CodeSources
    {
        get
        {
            List<DocsCodeSource> sources = new()
            {
                new( Code, CodeTitle )
            };

            if ( AdditionalCodes is not null )
            {
                sources.AddRange( AdditionalCodes.Where( source => source is not null && !string.IsNullOrWhiteSpace( source.Code ) ) );
            }

            return sources;
        }
    }

    private bool HasMultipleCodeSources => CodeSources.Count > 1;

    private string CurrentCode { get; set; }

    [Inject] public INotificationService NotificationService { get; set; }

    [Inject] public IJSRuntime JSRuntime { get; set; }

    [Parameter] public string Code { get; set; }

    [Parameter] public string CodeTitle { get; set; } = "Example.razor";

    [Parameter] public IReadOnlyList<DocsCodeSource> AdditionalCodes { get; set; }

    [Parameter] public bool ShowCode { get; set; } = true;

    #endregion
}