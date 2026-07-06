#region Using directives
using Blazorise;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Registers a document-scoped font family for declarative PDF documents.
/// </summary>
public class PdfFont : ComponentBase
{
    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        DocumentContext?.RegisterFont( CreateFontFamily() );
    }

    private FontFamily CreateFontFamily()
    {
        if ( Font is not null )
            return Font;

        return new()
        {
            Name = Name,
            DisplayName = DisplayName,
            CssFamily = CssFamily,
            Regular = Regular,
            Bold = Bold,
            Italic = Italic,
            BoldItalic = BoldItalic,
            Visible = Visible,
        };
    }

    #endregion

    #region Parameters

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