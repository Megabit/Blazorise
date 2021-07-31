using System;
using System.Reflection;

namespace Blazorise.DataGrid.Utils
{
    public static class RecursiveObjectActivator
    {
        /// <summary>
        /// Creates an instance of a complex object and it's public instance properties.
        /// Ignores lists, valuetypes and strings
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        public static TItem CreateInstance<TItem>()
        {
            var obj = Activator.CreateInstance<TItem>();
            var properties = typeof( TItem ).GetProperties( BindingFlags.Public | BindingFlags.Instance );
            CreateInstanceRecursive( obj, properties );

            return obj;
        }

        private static void CreateInstanceRecursive( object currObjInstance, PropertyInfo[] properties )
        {
            foreach ( var property in properties )
            {
                var currType = property.PropertyType;
                if ( !currType.IsValueType && currType != typeof( string ) && !TypeChecker.IsListOrCollection( currType ) )
                {
                    var instanced = property.GetValue( currObjInstance );
                    if ( instanced is null )
                    {
                        instanced = Activator.CreateInstance( currType );
                        property.SetValue( currObjInstance, instanced );
                    }
                    CreateInstanceRecursive( instanced, currType.GetProperties( BindingFlags.Public | BindingFlags.Instance ) );
                }
            }
        }
    }
}