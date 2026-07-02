#region Using directives
using System;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDataReferenceUpdater
{
    #region Methods

    internal static void ReplaceFormulaFieldReferences( ReportDefinition definition, string oldName, string newName )
    {
        if ( definition is null )
            return;

        foreach ( ReportFormulaFieldDefinition formulaField in definition.FormulaFields ?? [] )
        {
            formulaField.Formula = ReplaceFormulaFieldExpressionToken( formulaField.Formula, oldName, newName );
        }

        foreach ( ReportSectionDefinition section in definition.Sections ?? [] )
        {
            ReplaceFormulaFieldReference( section.Suppress, oldName, newName );
            ReplaceFormulaFieldReference( section.KeepTogether, oldName, newName );
            ReplaceFormulaFieldReference( section.NewPageBefore, oldName, newName );
            ReplaceFormulaFieldReference( section.NewPageAfter, oldName, newName );

            foreach ( ReportElementDefinition element in section.Elements ?? [] )
            {
                if ( element is ReportFieldElementDefinition fieldElement
                    && !string.IsNullOrWhiteSpace( fieldElement.Field )
                    && string.Equals( ReportFormulaFieldResolver.NormalizeFieldName( fieldElement.Field ), oldName, StringComparison.OrdinalIgnoreCase )
                    && ( ReportFormulaFieldResolver.IsFormulaDataSource( fieldElement.DataSource ) || string.IsNullOrWhiteSpace( fieldElement.DataSource ) ) )
                {
                    fieldElement.Field = newName;
                }

                if ( element is ReportTextElementDefinition textElement )
                    textElement.Text = ReplaceFormulaFieldExpressionToken( textElement.Text, oldName, newName );

                ReplaceFormulaFieldReference( element.CanGrow, oldName, newName );
                ReplaceFormulaFieldReference( element.Suppress, oldName, newName );
                ReplaceFormulaFieldReference( element.SnapToGrid, oldName, newName );
            }
        }
    }

    internal static void ReplaceRunningTotalReferences( ReportDefinition definition, string oldName, string newName )
    {
        if ( definition is null )
            return;

        foreach ( ReportFormulaFieldDefinition formulaField in definition.FormulaFields ?? [] )
        {
            formulaField.Formula = ReplaceRunningTotalExpressionToken( formulaField.Formula, oldName, newName );
        }

        foreach ( ReportRunningTotalDefinition runningTotal in definition.RunningTotals ?? [] )
        {
            runningTotal.EvaluateFormula = ReplaceRunningTotalExpressionToken( runningTotal.EvaluateFormula, oldName, newName );
        }

        foreach ( ReportSectionDefinition section in definition.Sections ?? [] )
        {
            ReplaceRunningTotalReference( section.Suppress, oldName, newName );
            ReplaceRunningTotalReference( section.KeepTogether, oldName, newName );
            ReplaceRunningTotalReference( section.NewPageBefore, oldName, newName );
            ReplaceRunningTotalReference( section.NewPageAfter, oldName, newName );

            foreach ( ReportElementDefinition element in section.Elements ?? [] )
            {
                if ( element is ReportFieldElementDefinition fieldElement
                    && !string.IsNullOrWhiteSpace( fieldElement.Field )
                    && string.Equals( ReportRunningTotalResolver.NormalizeFieldName( fieldElement.Field ), oldName, StringComparison.OrdinalIgnoreCase )
                    && ( ReportRunningTotalResolver.IsRunningTotalDataSource( fieldElement.DataSource ) || string.IsNullOrWhiteSpace( fieldElement.DataSource ) ) )
                {
                    fieldElement.Field = newName;
                }

                if ( element is ReportTextElementDefinition textElement )
                    textElement.Text = ReplaceRunningTotalExpressionToken( textElement.Text, oldName, newName );

                ReplaceRunningTotalReference( element.CanGrow, oldName, newName );
                ReplaceRunningTotalReference( element.Suppress, oldName, newName );
                ReplaceRunningTotalReference( element.SnapToGrid, oldName, newName );
            }
        }
    }

    private static void ReplaceFormulaFieldReference( ReportValue<bool> value, string oldName, string newName )
    {
        if ( value is not null )
            value.Formula = ReplaceFormulaFieldExpressionToken( value.Formula, oldName, newName );
    }

    private static void ReplaceFormulaFieldReference( ReportValue<bool?> value, string oldName, string newName )
    {
        if ( value is not null )
            value.Formula = ReplaceFormulaFieldExpressionToken( value.Formula, oldName, newName );
    }

    private static string ReplaceFormulaFieldExpressionToken( string value, string oldName, string newName )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
            return value;

        string oldToken = ReportExpressionFormatter.FormatFieldExpression( null, oldName );
        string newToken = ReportExpressionFormatter.FormatFieldExpression( null, newName );
        string oldQualifiedToken = $"{{{ReportFormulaFieldResolver.DataSourceName}.{oldName}}}";

        return value
            .Replace( oldQualifiedToken, newToken, StringComparison.OrdinalIgnoreCase )
            .Replace( oldToken, newToken, StringComparison.OrdinalIgnoreCase );
    }

    private static void ReplaceRunningTotalReference( ReportValue<bool> value, string oldName, string newName )
    {
        if ( value is not null )
            value.Formula = ReplaceRunningTotalExpressionToken( value.Formula, oldName, newName );
    }

    private static void ReplaceRunningTotalReference( ReportValue<bool?> value, string oldName, string newName )
    {
        if ( value is not null )
            value.Formula = ReplaceRunningTotalExpressionToken( value.Formula, oldName, newName );
    }

    private static string ReplaceRunningTotalExpressionToken( string value, string oldName, string newName )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
            return value;

        string oldToken = ReportExpressionFormatter.FormatFieldExpression( null, oldName );
        string newToken = ReportExpressionFormatter.FormatFieldExpression( null, newName );
        string oldQualifiedToken = $"{{{ReportRunningTotalResolver.DataSourceName}.{oldName}}}";

        return value
            .Replace( oldQualifiedToken, newToken, StringComparison.OrdinalIgnoreCase )
            .Replace( oldToken, newToken, StringComparison.OrdinalIgnoreCase );
    }

    #endregion
}