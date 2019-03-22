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
    }
}
