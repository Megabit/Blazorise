#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

internal delegate void ReportParameterHashBuilder<in T>( T value, ref HashCode hash );

internal static class ReportParameterViewExtensions
{
    public static bool TryAddHash<T>( this ParameterView parameters, string name, ref HashCode hash, ReportParameterHashBuilder<T> addHash )
    {
        if ( parameters.TryGetValue<T>( name, out T value ) )
        {
            addHash( value, ref hash );
            return true;
        }

        return false;
    }
}