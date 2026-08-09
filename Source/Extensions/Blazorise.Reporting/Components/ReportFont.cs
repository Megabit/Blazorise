#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Registers a report-scoped font family for declarative reports.
/// </summary>
public class ReportFont : ComponentBase, IDisposable
{
    #region Members

    private ReportContext registeredReportContext;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        bool definitionChanged = registeredReportContext is null
            || parameters.IsParameterChanged( Font )
            || parameters.IsParameterChanged( Name )
            || parameters.IsParameterChanged( DisplayName )
            || parameters.IsParameterChanged( CssFamily )
            || parameters.IsParameterChanged( Regular )
            || parameters.IsParameterChanged( Bold )
            || parameters.IsParameterChanged( Italic )
            || parameters.IsParameterChanged( BoldItalic )
            || parameters.IsParameterChanged( Visible );

        await base.SetParametersAsync( parameters );

        bool contextChanged = !ReferenceEquals( registeredReportContext, ReportContext );

        if ( contextChanged )
        {
            registeredReportContext?.UnregisterFont( this );
            registeredReportContext = ReportContext;
        }

        if ( definitionChanged || contextChanged )
            registeredReportContext?.RegisterFont( this, CreateFontFamily() );
    }

    /// <inheritdoc />
    public void Dispose()
    {
        registeredReportContext?.UnregisterFont( this );
        registeredReportContext = null;
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

    #region Properties

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