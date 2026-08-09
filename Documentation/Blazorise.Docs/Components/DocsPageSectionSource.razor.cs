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
    #region Members

    private bool expandedApplied;

    #endregion

    #region Methods

    protected override void OnParametersSet()
    {
        if ( !expandedApplied )
        {
            SourceExpanded = Expanded;
            expandedApplied = true;
        }

        if ( string.IsNullOrWhiteSpace( CurrentCode ) || !CodeSources.Any( source => source.Code == CurrentCode ) )
        {
            CurrentCode = Code;
            RequestSourceOverflowMeasure();
        }
    }

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( ShowCode && !SourceExpanded && shouldMeasureSourceOverflow )
        {
            shouldMeasureSourceOverflow = false;

            try
            {
                bool canExpandSource = await JSRuntime.InvokeAsync<bool>( "blazoriseDocs.code.hasVerticalOverflow", SourceCodeElementId );

                if ( CanExpandSource != canExpandSource )
                {
                    CanExpandSource = canExpandSource;
                    await InvokeAsync( StateHasChanged );
                }
            }
            catch ( JSException )
            {
            }
            catch ( InvalidOperationException )
            {
            }
        }
    }

    private Task OnToggleCode()
    {
        ShowCode = !ShowCode;

        if ( ShowCode )
        {
            SourceExpanded = false;
            RequestSourceOverflowMeasure();
        }

        return Task.CompletedTask;
    }

    private Task OnToggleSourceExpanded()
    {
        SourceExpanded = !SourceExpanded;

        if ( !SourceExpanded )
        {
            RequestSourceOverflowMeasure();
        }

        return Task.CompletedTask;
    }

    private async Task OnCopyCode()
    {
        await JSRuntime.InvokeVoidAsync( "blazoriseDocs.code.copyToClipboard", Snippets.GetCode( CopyFullExample ? Code : CurrentCode ) );
        await NotificationService.Info( CopyFullExample ? "Copied full example!" : "Copied code example!" );
    }

    private Task OnSelectCodeSource( string code )
    {
        if ( CurrentCode != code )
        {
            CurrentCode = code;
            SourceExpanded = false;
            RequestSourceOverflowMeasure();
        }

        return Task.CompletedTask;
    }

    private void RequestSourceOverflowMeasure()
    {
        if ( !SourceExpanded )
        {
            CanExpandSource = false;
        }

        shouldMeasureSourceOverflow = true;
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

    private IFluentSpacing SourceActionEndMargin => CanExpandSource ? Margin.Is2.FromEnd : null;

    private bool SourceExpanded { get; set; }

    private bool CanExpandSource { get; set; }

    private bool shouldMeasureSourceOverflow = true;

    private string SourceCodeElementId { get; } = $"b-docs-page-section-source-{Guid.NewGuid():N}";

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

    [Parameter] public bool CopyFullExample { get; set; }

    [Parameter] public bool ShowCode { get; set; } = true;

    /// <summary>
    /// Specifies whether the source code block is expanded by default.
    /// </summary>
    [Parameter] public bool Expanded { get; set; }

    #endregion
}