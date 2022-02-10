#region Using directives
using System;
using System.Reflection;
using System.Runtime.Serialization;
#endregion

namespace Blazorise.DataGrid.Utils
{
    public static class RecursiveObjectActivator
    {
        #region Methods

        /// <summary>
        /// Sets the maximum Depth permitted to recursively traverse the object graph.
        /// </summary>
        private readonly static int maxDepth = 64;

        /// <summary>
        /// Sets the maximum Circular Reference permitted, once it has been detected by the algorithm.
        /// </summary>
        private readonly static int maxCircularReferenceLevel = 1;

        /// <summary>
        /// Creates an instance of a complex object and it's public instance properties.
        /// Ignores lists, valuetypes and strings
        /// </summary>
        /// <typeparam name="TItem">The type to create.</typeparam>
        /// <returns>A reference to the newly created object.</returns>
        public static TItem CreateInstance<TItem>()
        {
            var objType = typeof( TItem );
            var obj = (TItem)FormatterServices.GetUninitializedObject( objType );
            var properties = objType.GetProperties( BindingFlags.Public | BindingFlags.Instance );
            CreateInstanceRecursive( obj, properties, (null, objType) );

            return obj;
        }

        private static void CreateInstanceRecursive( object currObjInstance, PropertyInfo[] properties, (Type previousParentType, Type parentType) typeTracker, int depthLevel = 0, int circularReferenceLevel = 0 )
        {
            //Possible object cycle detected
            if ( depthLevel == maxDepth )
                return;

            foreach ( var property in properties )
            {
                var currType = property.PropertyType;

                //Circular reference detected for this traversal
                if ( circularReferenceLevel == maxCircularReferenceLevel && ( typeTracker.parentType == currType || typeTracker.previousParentType == currType ) )
                    continue;

                if ( !currType.IsValueType && currType != typeof( string ) && !currType.IsCollection() )
                {
                    //Do not mutate circularReferenceLevel value
                    int propertyCircularReferenceLevel = circularReferenceLevel;
                    var instanced = property.GetValue( currObjInstance );
                    if ( instanced is null )
                    {
                        instanced = FormatterServices.GetUninitializedObject( currType );
                        property.SetValue( currObjInstance, instanced );
                    }
                    if ( typeTracker.parentType == currType || typeTracker.previousParentType == currType )
                        propertyCircularReferenceLevel++;

                    CreateInstanceRecursive( instanced, currType.GetProperties( BindingFlags.Public | BindingFlags.Instance ), (typeTracker.parentType, currType), ++depthLevel, propertyCircularReferenceLevel );
                }
            }
        }

        #endregion
    }
}