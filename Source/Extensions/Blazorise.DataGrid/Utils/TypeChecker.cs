using System;
using System.Collections.Generic;

namespace Blazorise.DataGrid.Utils
{
    public static class TypeChecker
    {
        public static bool IsListOrCollection( Type type )
            => typeof( System.Collections.IList ).IsAssignableFrom( type ) ||
                typeof( System.Collections.ICollection ).IsAssignableFrom( type ) ||
                typeof( IEnumerable<> ).IsAssignableFrom( type );
    }
}