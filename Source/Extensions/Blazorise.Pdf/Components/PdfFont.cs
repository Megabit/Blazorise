#region Using directives
using System;
using Blazorise;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Registers a document-scoped font family for declarative PDF documents.
/// </summary>
public class PdfFont : ComponentBase, IDisposable
{
    #region Members

    private FontFamily definition;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if ( definition is null )
        {
            if ( DocumentContext is null )
                return;

            definition = new();
        }

        UpdateDefinition();

        if ( string.IsNullOrWhiteSpace( definition.Name ) )
        {
            DocumentContext.UnregisterFont( definition );
            return;
        }

        DocumentContext.RegisterFont( definition );
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
    }

    /// <inheritdoc />
    public void Dispose()
    {
        DocumentContext?.UnregisterFont( definition );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Provides the current PDF document that receives this font registration.
    /// </summary>
    [CascadingParameter] protected PdfDocumentContext DocumentContext { get; set; }

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