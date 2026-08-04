#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Registers a document-scoped font family for declarative PDF documents.
/// </summary>
public class PdfFont : ComponentBase, IDisposable
{
    #region Members

    private readonly FontFamily definition = new();

    private PdfDocumentContext documentContext;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        bool definitionChanged = parameters.TryGetValue<FontFamily>( nameof( Font ), out _ )
            || parameters.IsParameterChanged( Name )
            || parameters.IsParameterChanged( DisplayName )
            || parameters.IsParameterChanged( CssFamily )
            || parameters.TryGetValue<FontSource>( nameof( Regular ), out _ )
            || parameters.TryGetValue<FontSource>( nameof( Bold ), out _ )
            || parameters.TryGetValue<FontSource>( nameof( Italic ), out _ )
            || parameters.TryGetValue<FontSource>( nameof( BoldItalic ), out _ )
            || parameters.IsParameterChanged( Visible );

        Task task = base.SetParametersAsync( parameters );

        if ( definitionChanged )
            UpdateDefinition();

        return task;
    }

    private void UpdateDefinition()
    {
        definition.Name = Font is null ? Name : Font.Name;
        definition.DisplayName = Font is null ? DisplayName : Font.DisplayName;
        definition.CssFamily = Font is null ? CssFamily : Font.CssFamily;
        definition.Regular = Font is null ? Regular : Font.Regular;
        definition.Bold = Font is null ? Bold : Font.Bold;
        definition.Italic = Font is null ? Italic : Font.Italic;
        definition.BoldItalic = Font is null ? BoldItalic : Font.BoldItalic;
        definition.Visible = Font is null ? Visible : Font.Visible;

        if ( string.IsNullOrWhiteSpace( definition.Name ) )
            documentContext?.UnregisterFont( definition );
        else
            documentContext?.RegisterFont( definition );
    }

    /// <inheritdoc />
    public void Dispose()
    {
        documentContext?.UnregisterFont( definition );
        documentContext = null;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Provides the current PDF document that receives this font registration.
    /// </summary>
    [CascadingParameter]
    protected PdfDocumentContext DocumentContext
    {
        get => documentContext;
        set
        {
            if ( ReferenceEquals( documentContext, value ) )
                return;

            documentContext?.UnregisterFont( definition );
            documentContext = value;
            UpdateDefinition();
        }
    }

    /// <summary>
    /// Complete font family registration.
    /// </summary>
    [Parameter] public FontFamily Font { get; set; }

    /// <summary>
    /// Font family name used by PDF elements.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// User-facing font family name.
    /// </summary>
    [Parameter] public string DisplayName { get; set; }

    /// <summary>
    /// CSS font-family value used by browser-based rendering.
    /// </summary>
    [Parameter] public string CssFamily { get; set; }

    /// <summary>
    /// Regular font source.
    /// </summary>
    [Parameter] public FontSource Regular { get; set; }

    /// <summary>
    /// Bold font source.
    /// </summary>
    [Parameter] public FontSource Bold { get; set; }

    /// <summary>
    /// Italic font source.
    /// </summary>
    [Parameter] public FontSource Italic { get; set; }

    /// <summary>
    /// Bold italic font source.
    /// </summary>
    [Parameter] public FontSource BoldItalic { get; set; }

    /// <summary>
    /// Indicates whether the font is visible in UI selectors.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    #endregion
}