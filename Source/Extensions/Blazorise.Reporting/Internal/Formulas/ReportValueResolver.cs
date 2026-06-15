#region Using directives
using System;
using System.Globalization;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportValueResolver
{
    #region Methods

    internal static T Resolve<T>( ReportValue<T> value, ReportFormulaContext context )
    {
        if ( value is null )
            return default;

        if ( !value.HasFormula )
            return value.Value;

        try
        {
            var formulaValue = ReportFormulaEvaluator.Evaluate( value.Formula, context );

            if ( TryConvert( formulaValue, out T convertedValue ) )
                return convertedValue;
        }
        catch
        {
        }

        return value.Value;
    }

    internal static bool ResolveSuppress( ReportSectionDefinition section, ReportDefinition definition, object data, object item )
    {
        return Resolve( section?.Suppress, new()
        {
            Definition = definition,
            Data = data,
            Item = item,
            Section = section,
        } );
    }

    internal static bool ResolveSuppress( ReportElementDefinition element, ReportSectionDefinition section, ReportDefinition definition, object data, object item )
    {
        return Resolve( element?.Suppress, new()
        {
            Definition = definition,
            Data = data,
            Item = item,
            Section = section,
            Element = element,
        } );
    }

    internal static bool ResolveCanGrow( ReportElementDefinition element, ReportSectionDefinition section, ReportDefinition definition, object data, object item, bool designMode )
    {
        if ( designMode )
            return element?.CanGrow?.Value ?? false;

        return Resolve( element?.CanGrow, new()
        {
            Definition = definition,
            Data = data,
            Item = item,
            Section = section,
            Element = element,
        } );
    }

    internal static bool? ResolveSnapToGrid( ReportElementDefinition element )
    {
        return element?.SnapToGrid?.Value;
    }

    private static bool TryConvert<T>( object value, out T convertedValue )
    {
        convertedValue = default;

        if ( value is null )
            return !typeof( T ).IsValueType || Nullable.GetUnderlyingType( typeof( T ) ) is not null;

        var targetType = Nullable.GetUnderlyingType( typeof( T ) ) ?? typeof( T );

        if ( value is T typedValue )
        {
            convertedValue = typedValue;
            return true;
        }

        try
        {
            if ( targetType.IsEnum )
            {
                convertedValue = (T)Enum.Parse( targetType, Convert.ToString( value, CultureInfo.InvariantCulture ), ignoreCase: true );
                return true;
            }

            convertedValue = (T)Convert.ChangeType( value, targetType, CultureInfo.CurrentCulture );
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion
}