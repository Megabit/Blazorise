#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportRunningTotalResolver
{
    #region Members

    internal const string DataSourceName = "__RunningTotals";

    #endregion

    #region Methods

    internal static ReportRunningTotalDefinition FindRunningTotal( ReportDefinition definition, string fieldName )
    {
        if ( definition?.RunningTotals is null || string.IsNullOrWhiteSpace( fieldName ) )
            return null;

        string normalizedFieldName = NormalizeFieldName( fieldName );

        return definition.RunningTotals.FirstOrDefault( runningTotal =>
            string.Equals( runningTotal.Name, normalizedFieldName, StringComparison.OrdinalIgnoreCase ) );
    }

    internal static string NormalizeFieldName( string fieldName )
    {
        return fieldName?.Trim();
    }

    internal static bool IsRunningTotalDataSource( string dataSource )
    {
        return string.Equals( dataSource, DataSourceName, StringComparison.OrdinalIgnoreCase );
    }

    internal static bool IsRunningTotalField( ReportDefinition definition, string fieldName )
    {
        return FindRunningTotal( definition, fieldName ) is not null;
    }

    internal static bool TryResolve( string dataSource, string fieldName, IReadOnlyDictionary<string, object> values, out object value )
    {
        value = null;

        if ( values is null || string.IsNullOrWhiteSpace( fieldName ) )
            return false;

        return values.TryGetValue( NormalizeFieldName( fieldName ), out value );
    }

    internal static ReportRunningTotalState CreateState( ReportDefinition definition, object data )
    {
        return new( definition, data );
    }

    #endregion
}

internal sealed class ReportRunningTotalState
{
    #region Members

    private readonly ReportDefinition definition;

    private readonly object data;

    private readonly Dictionary<string, RunningTotalAccumulator> accumulators = new( StringComparer.OrdinalIgnoreCase );

    #endregion

    #region Constructors

    internal ReportRunningTotalState( ReportDefinition definition, object data )
    {
        this.definition = definition;
        this.data = data;

        foreach ( ReportRunningTotalDefinition runningTotal in definition?.RunningTotals ?? [] )
        {
            if ( string.IsNullOrWhiteSpace( runningTotal?.Name ) )
                continue;

            accumulators[runningTotal.Name.Trim()] = new( runningTotal.AggregateFunction );
        }
    }

    #endregion

    #region Methods

    internal IReadOnlyDictionary<string, object> BuildSnapshot()
    {
        return accumulators.ToDictionary(
            accumulator => accumulator.Key,
            accumulator => accumulator.Value.ResolveValue(),
            StringComparer.OrdinalIgnoreCase );
    }

    internal void ProcessSection( ReportRenderSection renderSection )
    {
        if ( definition?.RunningTotals is null || definition.RunningTotals.Count == 0 || renderSection?.Section is null )
            return;

        ResetForSection( renderSection.Section );

        if ( renderSection.Section.Type != ReportSectionType.Detail )
            return;

        foreach ( ReportRunningTotalDefinition runningTotal in definition.RunningTotals )
        {
            if ( runningTotal is null
                || string.IsNullOrWhiteSpace( runningTotal.Name )
                || !ShouldAccumulate( runningTotal, renderSection.Section, renderSection.Item ) )
            {
                continue;
            }

            object value = string.IsNullOrWhiteSpace( runningTotal.Field )
                ? renderSection.Item
                : ReportExpressionResolver.ResolveValue( definition, data, renderSection.Item, runningTotal.Field, ResolveDataSource( runningTotal, renderSection.Section ) );

            if ( accumulators.TryGetValue( runningTotal.Name.Trim(), out RunningTotalAccumulator accumulator ) )
                accumulator.Accumulate( runningTotal.AggregateFunction, value );
        }
    }

    private void ResetForSection( ReportSectionDefinition section )
    {
        if ( section.Type != ReportSectionType.GroupHeader )
            return;

        foreach ( ReportRunningTotalDefinition runningTotal in definition.RunningTotals )
        {
            if ( runningTotal.ResetMode != ReportRunningTotalResetMode.Group
                || !string.Equals( runningTotal.ResetGroupId, section.Id, StringComparison.Ordinal )
                || string.IsNullOrWhiteSpace( runningTotal.Name ) )
            {
                continue;
            }

            if ( accumulators.TryGetValue( runningTotal.Name.Trim(), out RunningTotalAccumulator accumulator ) )
                accumulator.Reset();
        }
    }

    private bool ShouldAccumulate( ReportRunningTotalDefinition runningTotal, ReportSectionDefinition section, object item )
    {
        if ( !IsDataSourceMatch( runningTotal, section ) )
            return false;

        if ( runningTotal.EvaluateMode != ReportRunningTotalEvaluateMode.Formula )
            return true;

        if ( string.IsNullOrWhiteSpace( runningTotal.EvaluateFormula ) )
            return false;

        object result = ReportFormulaEvaluator.Evaluate( runningTotal.EvaluateFormula, new()
        {
            Definition = definition,
            Data = data,
            Item = item,
            Section = section,
        } );

        return ConvertToBoolean( result );
    }

    private static bool IsDataSourceMatch( ReportRunningTotalDefinition runningTotal, ReportSectionDefinition section )
    {
        if ( string.IsNullOrWhiteSpace( runningTotal.DataSource ) )
            return true;

        if ( string.IsNullOrWhiteSpace( section.DataSource ) )
            return false;

        return string.Equals( runningTotal.DataSource.Trim(), section.DataSource.Trim(), StringComparison.OrdinalIgnoreCase );
    }

    private static string ResolveDataSource( ReportRunningTotalDefinition runningTotal, ReportSectionDefinition section )
    {
        return string.IsNullOrWhiteSpace( runningTotal.DataSource )
            ? section.DataSource
            : runningTotal.DataSource;
    }

    private static bool ConvertToBoolean( object value )
    {
        if ( value is bool boolValue )
            return boolValue;

        if ( value is null )
            return false;

        try
        {
            return Convert.ToBoolean( value, CultureInfo.CurrentCulture );
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Classes

    private sealed class RunningTotalAccumulator
    {
        #region Members

        private readonly ReportAggregateFunction function;

        private int count;

        private decimal sum;

        private object minimum;

        private object maximum;

        #endregion

        #region Constructors

        internal RunningTotalAccumulator( ReportAggregateFunction function )
        {
            this.function = function;
        }

        #endregion

        #region Methods

        internal void Accumulate( ReportAggregateFunction runningTotalFunction, object value )
        {
            if ( runningTotalFunction == ReportAggregateFunction.Count )
            {
                if ( value is not null )
                    count++;

                return;
            }

            if ( value is null )
                return;

            if ( runningTotalFunction is ReportAggregateFunction.Sum or ReportAggregateFunction.Average )
            {
                if ( TryConvertToDecimal( value, out decimal decimalValue ) )
                {
                    sum += decimalValue;
                    count++;
                }

                return;
            }

            if ( runningTotalFunction == ReportAggregateFunction.Minimum )
            {
                if ( minimum is null || CompareValues( value, minimum ) < 0 )
                    minimum = value;

                return;
            }

            if ( runningTotalFunction == ReportAggregateFunction.Maximum && ( maximum is null || CompareValues( value, maximum ) > 0 ) )
                maximum = value;
        }

        internal void Reset()
        {
            count = 0;
            sum = 0;
            minimum = null;
            maximum = null;
        }

        internal object ResolveValue()
        {
            return function switch
            {
                ReportAggregateFunction.Count => count,
                ReportAggregateFunction.Sum => sum,
                ReportAggregateFunction.Average => count == 0 ? null : sum / count,
                ReportAggregateFunction.Minimum => minimum,
                ReportAggregateFunction.Maximum => maximum,
                _ => null,
            };
        }

        private static int CompareValues( object value, object otherValue )
        {
            if ( TryConvertToDecimal( value, out decimal decimalValue ) && TryConvertToDecimal( otherValue, out decimal otherDecimalValue ) )
                return decimalValue.CompareTo( otherDecimalValue );

            return value is IComparable comparable
                ? comparable.CompareTo( otherValue )
                : string.Compare( Convert.ToString( value, CultureInfo.CurrentCulture ), Convert.ToString( otherValue, CultureInfo.CurrentCulture ), StringComparison.CurrentCulture );
        }

        private static bool TryConvertToDecimal( object value, out decimal decimalValue )
        {
            try
            {
                decimalValue = Convert.ToDecimal( value, CultureInfo.CurrentCulture );
                return true;
            }
            catch
            {
                decimalValue = 0;
                return false;
            }
        }

        #endregion
    }

    #endregion
}