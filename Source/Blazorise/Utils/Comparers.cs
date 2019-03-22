#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Utils
{
    public static class Comparers
    {
        public static int Compare( object a, object b )
        {
            var ac = a as IComparable;
            var bc = b as IComparable;

            if ( ac == null || bc == null )
                throw new NotSupportedException();

            return ac.CompareTo( bc );
        }

        public static bool AreEqual( Array array1, Array array2 )
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
