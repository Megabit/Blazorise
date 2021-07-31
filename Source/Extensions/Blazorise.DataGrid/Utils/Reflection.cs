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
        public static TItem InitObject<TItem>( string path = null )
        {
            Type objType = typeof( TItem );
            var obj = (TItem)Activator.CreateInstance( typeof( TItem ) );
            var properties = objType.GetProperties();
            InitPropertyIterator( obj, properties );

            return obj;
        }

        private static void InitPropertyIterator( object currObjInstance, PropertyInfo[] properties )
        {
            foreach ( var property in properties )
            {
                var currType = property.PropertyType;
                if ( !currType.IsValueType )
                {
                    var instanced = property.GetValue( currObjInstance );
                    if ( instanced is null )
                    {
                        instanced = Activator.CreateInstance( currType );
                        property.SetValue( currObjInstance, instanced );
                    }
                    InitPropertyIterator( instanced, currType.GetProperties() );
                }
            }
        }

    }
}
