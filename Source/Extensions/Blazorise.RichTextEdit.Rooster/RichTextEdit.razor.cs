using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.RichTextEdit.Rooster;

/// <summary>
/// RichTextEdit based on rooster.js
/// </summary>
public partial class RichTextEdit : BaseComponent
{
    private DotNetObjectReference<RoosterAdapter> adapter;

    /// <summary>
    /// Perform format action
    /// </summary>
    /// <param name="action">the action to perform <see cref="RichTextEditAction"/></param>
    /// <param name="args">action arguments</param>
    public ValueTask Format( object action, params object[] args ) => action switch
    {
        string actionString => JSModule.Format( ElementRef, ElementId, actionString, args ),
        RichTextEditAction.Bold => JSModule.Format( ElementRef, ElementId, "toggleBold", args ),
        RichTextEditAction.Italic => JSModule.Format( ElementRef, ElementId, "toggleItalic", args ),
        RichTextEditAction.Underline => JSModule.Format( ElementRef, ElementId, "toggleUnderline", args ),
        RichTextEditAction.Strike => JSModule.Format( ElementRef, ElementId, "toggleStrikethrough", args ),
        RichTextEditAction.Blockquote => JSModule.Format( ElementRef, ElementId, "toggleBlockQuote", args ),
        RichTextEditAction.CodeBlock => JSModule.Format( ElementRef, ElementId, "toggleCodeBlock", args ),
        RichTextEditAction.Header => JSModule.Format( ElementRef, ElementId, "toggleHeader", args ),
        RichTextEditAction.List => JSModule.Format( ElementRef, ElementId, "toggleBullet", args ),
        RichTextEditAction.Script => JSModule.Format( ElementRef, ElementId, "toggleBold", args ),
        RichTextEditAction.Indent => JSModule.Format( ElementRef, ElementId, "toggleBold", args ),
        RichTextEditAction.Direction => JSModule.Format( ElementRef, ElementId, "toggleBold", args ),
        RichTextEditAction.Size => JSModule.Format( ElementRef, ElementId, "toggleBold", args ),
        RichTextEditAction.Color => JSModule.Format( ElementRef, ElementId, "toggleBold", args ),
        RichTextEditAction.Background => JSModule.Format( ElementRef, ElementId, "toggleBold", args ),
        RichTextEditAction.Font => JSModule.Format( ElementRef, ElementId, "toggleBold", args ),
        RichTextEditAction.Align => JSModule.Format( ElementRef, ElementId, "toggleBold", args ),
        RichTextEditAction.Clean => JSModule.Format( ElementRef, ElementId, "toggleBold", args ),
        RichTextEditAction.Link => JSModule.Format( ElementRef, ElementId, "toggleBold", args ),
        RichTextEditAction.Image => JSModule.Format( ElementRef, ElementId, "toggleBold", args ),
        _ => throw new ArgumentOutOfRangeException( nameof( action ), action, null )
    };

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await base.OnAfterRenderAsync( firstRender );

        if ( firstRender )
        {
            JSModule ??= new JSRoosterModule( JSRuntime, VersionProvider );
            adapter = DotNetObjectReference.Create( new RoosterAdapter( this ) );

            await JSModule.Initialize( adapter, ElementRef, ElementId, default );
        }
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-rte-rooster" );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );

            await JSModule.SafeDisposeAsync();

            if ( adapter != null )
            {
                adapter.Dispose();
                adapter = null;
            }
        }

        await base.DisposeAsync( disposing );
    }

    internal JSRoosterModule JSModule { get; set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Inject] private IVersionProvider VersionProvider { get; set; }
}

