using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blazorise.DataGrid.Utils
{
    public static class Reflection
    {
        /// <summary>
        /// Inits a complex object and inner public instance properties.
        /// Ignores lists, valuetypes and strings
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        public static TItem InitObject<TItem>()
        {
            Type objType = typeof( TItem );
            var obj = (TItem)Activator.CreateInstance( typeof( TItem ) );
            var properties = objType.GetProperties( BindingFlags.Public | BindingFlags.Instance );
            InitPropertyIterator( obj, properties );

            return obj;
        }

        public static bool IsListOrCollection( Type type )
            => typeof( System.Collections.IList ).IsAssignableFrom( type ) ||
                typeof( System.Collections.ICollection ).IsAssignableFrom( type ) ||
                typeof( IEnumerable<> ).IsAssignableFrom( type );

        private static void InitPropertyIterator( object currObjInstance, PropertyInfo[] properties )
        {
            foreach ( var property in properties )
            {
                var currType = property.PropertyType;
                if ( !currType.IsValueType && currType != typeof( string ) && !IsListOrCollection( currType ) )
                {
                    var instanced = property.GetValue( currObjInstance );
                    if ( instanced is null )
                    {
                        instanced = Activator.CreateInstance( currType );
                        property.SetValue( currObjInstance, instanced );
                    }
                    InitPropertyIterator( instanced, currType.GetProperties( BindingFlags.Public | BindingFlags.Instance ) );
                }
            }
        }

    }
}
