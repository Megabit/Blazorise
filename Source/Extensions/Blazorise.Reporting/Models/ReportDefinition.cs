#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Describes a complete report document, including page setup, data sources, and bands.
/// </summary>
public sealed class ReportDefinition
{
    /// <summary>
    /// Adds a report-scoped font family.
    /// </summary>
    /// <param name="font">Font family.</param>
    /// <returns>The report definition.</returns>
    public ReportDefinition AddFont( FontFamily font )
    {
        if ( string.IsNullOrWhiteSpace( font?.Name ) )
            return this;

        Fonts ??= [];

        int existingIndex = Fonts.FindIndex( x => string.Equals( x.Name, font.Name, StringComparison.OrdinalIgnoreCase ) );

        if ( existingIndex >= 0 )
            Fonts[existingIndex] = font;
        else
            Fonts.Add( font );

        return this;
    }

    /// <summary>
    /// Adds a report-scoped font family.
    /// </summary>
    /// <param name="name">Font family name.</param>
    /// <param name="regular">Regular font source.</param>
    /// <returns>The report definition.</returns>
    public ReportDefinition AddFont( string name, FontSource regular )
    {
        return AddFont( name, regular, null, null, null );
    }

    /// <summary>
    /// Adds a report-scoped font family.
    /// </summary>
    /// <param name="name">Font family name.</param>
    /// <param name="regular">Regular font source.</param>
    /// <param name="bold">Bold font source.</param>
    /// <param name="italic">Italic font source.</param>
    /// <param name="boldItalic">Bold italic font source.</param>
    /// <returns>The report definition.</returns>
    public ReportDefinition AddFont( string name, FontSource regular, FontSource bold = null, FontSource italic = null, FontSource boldItalic = null )
    {
        return AddFont( new()
        {
            Name = name,
            Regular = regular,
            Bold = bold,
            Italic = italic,
            BoldItalic = boldItalic,
        } );
    }

    /// <summary>
    /// Stable identifier used by the designer and persisted report state.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    /// <summary>
    /// Friendly report name shown in designer surfaces.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Page setup used by preview and export renderers.
    /// </summary>
    public ReportPageDefinition Page { get; set; } = new();

    /// <summary>
    /// Data sources available to fields and detail bands.
    /// </summary>
    public List<ReportDataSourceDefinition> DataSources { get; set; } = [];

    /// <summary>
    /// Calculated fields that evaluate formulas at render time and can be placed like source fields.
    /// </summary>
    public List<ReportFormulaFieldDefinition> FormulaFields { get; set; } = [];

    /// <summary>
    /// Stateful summary fields that accumulate values while detail records are rendered.
    /// </summary>
    public List<ReportRunningTotalDefinition> RunningTotals { get; set; } = [];

    /// <summary>
    /// Ordered report bands that make up the document body.
    /// </summary>
    public List<ReportBandDefinition> Bands { get; set; } = [];

    /// <summary>
    /// Report-scoped font families resolved before globally registered fonts.
    /// </summary>
    public List<FontFamily> Fonts { get; set; } = [];
}