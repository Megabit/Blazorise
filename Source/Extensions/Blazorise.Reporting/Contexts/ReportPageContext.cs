#region Using directives
using System;
using System.Linq;
#endregion

namespace Blazorise.Reporting;

internal sealed class ReportPageContext
{
    private readonly ReportRegistrationCollection<ReportBandDefinition> bands = new();

    #region Constructors

    public ReportPageContext( ReportPageDefinition page, Action definitionChanged )
    {
        Page = page;
        DefinitionChanged = definitionChanged;
    }

    #endregion

    #region Methods

    public void RegisterBand( object owner, ReportBandDefinition band )
    {
        if ( string.IsNullOrWhiteSpace( band.Name ) )
            band.Name = band.Type.ToString();

        bands.Set( owner, band );
        Page.Bands = bands.Values.ToList();
        NotifyDefinitionChanged();
    }

    public void UnregisterBand( object owner )
    {
        if ( !bands.Remove( owner ) )
            return;

        Page.Bands = bands.Values.ToList();
        NotifyDefinitionChanged();
    }

    internal void NotifyDefinitionChanged()
    {
        DefinitionChanged?.Invoke();
    }

    #endregion

    #region Properties

    public ReportPageDefinition Page { get; }

    internal Action DefinitionChanged { get; set; }

    #endregion
}