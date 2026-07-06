#region Using directives
using Blazorise;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Registers a report-scoped font family for declarative reports.
/// </summary>
public class ReportFont : ComponentBase
{
    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        ReportContext?.RegisterFont( CreateFontFamily() );
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
    /// Provides the current declarative report context.
    /// </summary>
    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    /// <summary>
    /// Complete font family registration.
    /// </summary>
    [Parameter] public FontFamily Font { get; set; }

    /// <summary>
    /// Font family name used by report elements.
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