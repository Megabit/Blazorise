using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazorise.Extensions
{
    public static class ArrayExtensions
    {
        public static bool AreEqual<T>( this IEnumerable<T> array1, IEnumerable<T> array2 )
        {
            return AreArraysEqual( array1?.ToArray(), array2?.ToArray() );
        }

        public static bool AreArraysEqual( this Array array1, Array array2 )
        {
            if ( array1 == null && array2 == null )
                return true;

            if ( array1 != null && array2 != null
                && array1.Length == array2.Length )
            {
                for ( int i = 0; i < array1.Length; ++i )
                {
                    if ( array1.GetValue( i ) != array2.GetValue( i ) )
                        return false;
                }

                return true;
            }

            return false;
        }
    }
}
