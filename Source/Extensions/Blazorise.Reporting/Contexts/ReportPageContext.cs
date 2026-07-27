#region Using directives
using System;
using System.Linq;
#endregion

namespace Blazorise.Reporting;

internal sealed class ReportPageContext
{
    #region Constructors

    public ReportPageContext( ReportPageDefinition page )
    {
        Page = page;
    }

    #endregion

    #region Methods

    public ReportBandDefinition RegisterBand( ReportBandDefinition band )
    {
        if ( string.IsNullOrWhiteSpace( band.Name ) )
            band.Name = band.Type.ToString();

        ReportBandDefinition existing = Page.Bands.FirstOrDefault( x => string.Equals( x.Name, band.Name, StringComparison.OrdinalIgnoreCase ) );

        if ( existing is null )
        {
            Page.Bands.Add( band );
            return band;
        }

        existing.Type = band.Type;
        existing.Height = band.Height;
        existing.DataSource = band.DataSource;
        existing.Class = band.Class;
        existing.Style = band.Style;
        existing.Default = band.Default;
        existing.Suppress = band.Suppress;
        existing.ReserveSpaceWhenSuppressed = band.ReserveSpaceWhenSuppressed;
        existing.PrintOnFirstPage = band.PrintOnFirstPage;
        existing.PrintOnLastPage = band.PrintOnLastPage;
        existing.RepeatOnEveryPage = band.RepeatOnEveryPage;
        existing.KeepTogether = band.KeepTogether;
        existing.NewPageBefore = band.NewPageBefore;
        existing.NewPageAfter = band.NewPageAfter;
        existing.Appearance = band.Appearance;
        existing.Border = band.Border;
        existing.Elements.Clear();

        return existing;
    }

    #endregion

    #region Properties

    public ReportPageDefinition Page { get; }

    #endregion
}