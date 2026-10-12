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

    private bool shouldMeasureSourceOverflow = true;

    #endregion

    #region Methods

    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var codeTitleChanged = parameters.TryGetValue<string>( nameof( CodeTitle ), out var paramCodeTitle ) && paramCodeTitle != CodeTitle;
        var codeChanged = parameters.TryGetValue<string>( nameof( Code ), out var paramCode ) && paramCode != Code;
        var showCodeChanged = parameters.TryGetValue<bool>( nameof( ShowCode ), out var paramShowCode ) && paramShowCode != ShowCode;
        var copyFullExampleChanged = parameters.TryGetValue<bool>( nameof( CopyFullExample ), out var paramCopyFullExample ) && paramCopyFullExample != CopyFullExample;

        await base.SetParametersAsync( parameters );

        if ( showCodeChanged )
        {
            IsCodeVisible = ShowCode;
        }

        if ( codeChanged || showCodeChanged )
        {
            RequestSourceOverflowMeasure();
        }

        if ( codeTitleChanged || codeChanged || showCodeChanged || copyFullExampleChanged )
        {
            ParentSourceGroup?.NotifySourceChanged( this );
        }
    }

    protected override void OnInitialized()
    {
        ParentSourceGroup?.RegisterSource( this );

        base.OnInitialized();
    }

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
        if ( ParentSourceGroup is null )
        {
            await MeasureSourceOverflowAsync();
        }
    }

    public void Dispose()
    {
        ParentSourceGroup?.UnregisterSource( this );
    }

    internal async Task MeasureSourceOverflowAsync()
    {
        if ( IsCodeVisible && !SourceExpanded && shouldMeasureSourceOverflow )
        {
            shouldMeasureSourceOverflow = false;

            try
            {
                var canExpandSource = await JSRuntime.InvokeAsync<bool>( "blazoriseDocs.code.hasVerticalOverflow", SourceCodeElementId );

                if ( CanExpandSource != canExpandSource )
                {
                    CanExpandSource = canExpandSource;
                    ParentSourceGroup?.NotifySourceChanged( this );
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

    private Task OnToggleCodeHandler()
    {
        IsCodeVisible = !IsCodeVisible;

        if ( IsCodeVisible )
        {
            SourceExpanded = false;
            RequestSourceOverflowMeasure();
        }

        ParentSourceGroup?.NotifySourceChanged( this );

        return Task.CompletedTask;
    }

    private Task OnToggleSourceExpandedHandler()
    {
        SourceExpanded = !SourceExpanded;

        if ( !SourceExpanded )
        {
            RequestSourceOverflowMeasure();
        }

        ParentSourceGroup?.NotifySourceChanged( this );

        return Task.CompletedTask;
    }

    private async Task OnCopyCodeHandler()
    {
        await JSRuntime.InvokeVoidAsync( "blazoriseDocs.code.copyToClipboard", Snippets.GetCode( CopyFullExample ? Code : CurrentCode ) );
        await NotificationService.Info( CopyFullExample ? "Copied full example!" : "Copied code example!" );
    }

    private Task OnSelectCodeSourceHandler( string code )
    {
        if ( CurrentCode != code )
        {
            CurrentCode = code;
            SourceExpanded = false;
            RequestSourceOverflowMeasure();
        }

        ParentSourceGroup?.NotifySourceChanged( this );

        return Task.CompletedTask;
    }

    internal void RequestSourceOverflowMeasure()
    {
        if ( !SourceExpanded )
        {
            CanExpandSource = false;
        }

        shouldMeasureSourceOverflow = true;
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

    private IFluentDisplay SourceCodeDisplay => IsCodeVisible ? Display.Block : Display.None;

    private IFluentSizing SourceCodeHeight => SourceExpanded ? null : Height.Px().Max( 280 );

    private IFluentSizing SourceActionWidth => HasSourceSelector ? Width.Rem( 2.5 ) : null;

    private IFluentSizing SourceActionHeight => HasSourceSelector ? Height.Rem( 2.5 ) : null;

    private IFluentSpacing SourceActionEndMargin => CanExpandSource && !HasSourceSelector ? Margin.Is2.FromEnd : null;

    private bool SourceExpanded { get; set; }

    private bool IsCodeVisible { get; set; } = true;

    private bool CanExpandSource { get; set; }

    internal string SourceCodeElementId { get; } = $"b-docs-page-section-source-{Guid.NewGuid():N}";

    private IReadOnlyList<DocsCodeSource> CodeSources
    {
        get
        {
            var sources = new List<DocsCodeSource>
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

    private bool HasSourceSelector => ParentSourceGroup is not null || HasMultipleCodeSources;

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

    [Parameter] public RenderFragment ChildContent { get; set; }

    [CascadingParameter] protected DocsPageSectionSourceGroup ParentSourceGroup { get; set; }

    #endregion
}