using System.Collections.Generic;

namespace Blazorise.Extensions
{
    public static class DictionaryExtensions
    {
        public static T GetAndRemove<T>( this IDictionary<string, object> values, string key )
        {
            if ( values.TryGetValue( key, out var value ) )
            {
                values.Remove( key );
                return (T)value;
            }
            else
            {
                return default;
            }
        }
    }
}
