#region Using directives
using System;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDataDefinitionService
{
    #region Methods

    internal ReportFormulaFieldDefinition FindFormulaField( ReportDefinition definition, string formulaFieldName )
    {
        if ( definition?.FormulaFields is null || string.IsNullOrWhiteSpace( formulaFieldName ) )
            return null;

        string normalizedFormulaFieldName = ReportFormulaFieldResolver.NormalizeFieldName( formulaFieldName );

        return definition.FormulaFields.FirstOrDefault( field =>
            string.Equals( field.Name, normalizedFormulaFieldName, StringComparison.OrdinalIgnoreCase ) );
    }

    internal ReportRunningTotalDefinition FindRunningTotal( ReportDefinition definition, string runningTotalName )
    {
        return ReportRunningTotalResolver.FindRunningTotal( definition, runningTotalName );
    }

    internal ReportDataSourceDefinition FindDataSource( ReportDefinition definition, string dataSourceName )
    {
        if ( definition?.DataSources is null || string.IsNullOrWhiteSpace( dataSourceName ) )
            return null;

        return definition.DataSources.FirstOrDefault( dataSource =>
            string.Equals( dataSource.Name, dataSourceName, StringComparison.OrdinalIgnoreCase ) );
    }

    internal int GetInsertionSectionIndex( ReportDefinition definition, int? selectedSectionIndex, string selectedElementKey )
    {
        if ( definition?.Sections is null )
            return -1;

        if ( selectedSectionIndex is { } sectionIndex
            && sectionIndex >= 0
            && sectionIndex < definition.Sections.Count )
        {
            return sectionIndex;
        }

        if ( !string.IsNullOrWhiteSpace( selectedElementKey )
            && ReportDefinitionHelper.TryFindElementLocation( definition, selectedElementKey, out int elementSectionIndex, out _, out _ ) )
        {
            return elementSectionIndex;
        }

        return -1;
    }

    internal double GetNextInsertionY( ReportSectionDefinition section, Func<double, double> applyGrid )
    {
        if ( section?.Elements is null || section.Elements.Count == 0 )
            return 0;

        double y = section.Elements.Max( element => element.Y + element.Height ) + ReportDesignerConstants.DefaultDroppedFieldHeight;

        return applyGrid( y );
    }

    #endregion
}