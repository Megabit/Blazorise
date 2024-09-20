using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Extensions;

/// <summary>
/// Helper methods for component parameters.
/// </summary>
public static class ParameterViewExtensions
{
    /// <summary>
    /// Gets the value of the parameter with the specified name.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="parameters">Dictionary of all component paremeters.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="currentValue">Last known parameter value.</param>
    /// <param name="result">Receives the result, if any.</param>
    public static bool TryGetParameter<T>( this ParameterView parameters, string parameterName, T currentValue, out ComponentParameterInfo<T> result )
    {
        if ( parameters.TryGetValue<T>( parameterName, out var paramNewValue ) )
        {
            var changed = currentValue is IEnumerable<T> array && paramNewValue is IEnumerable<T> array2
                ? !array.AreEqual( array2 )
                : !paramNewValue.IsEqual( currentValue );

            result = new ComponentParameterInfo<T>( paramNewValue, true, changed );
        }
        else
            result = new ComponentParameterInfo<T>( default );

        return result.Defined;
    }
}