#region Using directives
using System;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportFormulaFieldResolver
{
    #region Members

    internal const string DataSourceName = "__FormulaFields";

    #endregion

    #region Methods

    internal static bool IsFormulaDataSource( string dataSourceName )
    {
        return string.Equals( dataSourceName, DataSourceName, StringComparison.OrdinalIgnoreCase );
    }

    internal static bool IsFormulaField( ReportDefinition definition, string fieldName )
    {
        return FindFormulaField( definition, fieldName ) is not null;
    }

    internal static bool TryResolve( string dataSourceName, string fieldName, ReportDefinition definition, object data, object item, ReportBandDefinition section, out object value )
    {
        value = null;

        if ( string.IsNullOrWhiteSpace( fieldName ) )
            return false;

        if ( !IsFormulaDataSource( dataSourceName ) && !IsFormulaField( definition, fieldName ) )
            return false;

        ReportFormulaFieldDefinition formulaField = FindFormulaField( definition, fieldName );

        if ( formulaField is null )
            return false;

        value = ReportFormulaEvaluator.Evaluate( formulaField.Formula, new()
        {
            Definition = definition,
            Data = data,
            Item = item,
            Section = section,
        } );

        return true;
    }

    internal static ReportFormulaFieldDefinition FindFormulaField( ReportDefinition definition, string fieldName )
    {
        if ( definition?.FormulaFields is null || string.IsNullOrWhiteSpace( fieldName ) )
            return null;

        string normalizedFieldName = NormalizeFieldName( fieldName );

        return definition.FormulaFields.FirstOrDefault( field =>
            string.Equals( field?.Name, normalizedFieldName, StringComparison.OrdinalIgnoreCase ) );
    }

    internal static string NormalizeFieldName( string fieldName )
    {
        string normalizedFieldName = fieldName.Trim();
        string dataSourcePrefix = $"{DataSourceName}.";

        return normalizedFieldName.StartsWith( dataSourcePrefix, StringComparison.OrdinalIgnoreCase )
            ? normalizedFieldName[dataSourcePrefix.Length..]
            : normalizedFieldName;
    }

    #endregion
}