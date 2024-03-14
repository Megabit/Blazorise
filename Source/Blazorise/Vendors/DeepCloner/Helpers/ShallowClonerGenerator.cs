using System;

namespace Blazorise.DeepCloner.Helpers;

internal static class ShallowClonerGenerator
{
    public static T CloneObject<T>( T obj )
    {
        // this is faster than typeof(T).IsValueType
        if ( obj is ValueType )
        {
            if ( typeof( T ) == obj.GetType() )
                return obj;

            // we're here so, we clone value type obj as object type T
            // so, we need to copy it, bcs we have a reference, not real object.
            return (T)ShallowObjectCloner.CloneObject( obj );
        }

        if ( ReferenceEquals( obj, null ) )
            return (T)(object)null;

        if ( DeepClonerSafeTypes.CanReturnSameObject( obj.GetType() ) )
            return obj;

        return (T)ShallowObjectCloner.CloneObject( obj );
    }
}
