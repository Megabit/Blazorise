#region Using directives
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

internal static class ReportParameterViewExtensions
{
    internal static bool IsReportValueChanged<T>( this ParameterView parameters, ReportValue<T> currentValue, [CallerArgumentExpression( "currentValue" )] string parameterName = null )
    {
        return parameters.TryGetParameter( currentValue,
            value => EqualityComparer<T>.Default.Equals( value is null ? default : value.Value, currentValue is null ? default : currentValue.Value )
                && string.Equals( value?.Formula, currentValue?.Formula, System.StringComparison.Ordinal ),
            out ComponentParameterInfo<ReportValue<T>> parameter,
            parameterName ) && parameter.Changed;
    }
}