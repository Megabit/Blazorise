#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Extensions;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public static class ParameterViewExtensions
{
    public static void TryGetParameter<T>( this ParameterView parameters, string parameterName, T currentValue, out ComponentParameterInfo<T> componentParameter )
    {
        if ( parameters.TryGetValue<T>( parameterName, out var paramNewValue ) )
            componentParameter = new ComponentParameterInfo<T>( paramNewValue, true, !paramNewValue.IsEqual( currentValue ) );
        else
            componentParameter = new ComponentParameterInfo<T>( default );
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member