﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
#endregion

namespace Blazorise.DataGrid.Utils
{
    static class FunctionCompiler
    {
        public static Func<TItem> CreateNewItem<TItem>()
        {
            return Expression.Lambda<Func<TItem>>( Expression.New( typeof( TItem ) ) ).Compile();
        }

        /// <summary>
        /// Bulds an access expression for nested properties while checking for null values.
        /// </summary>
        /// <param name="item">Item that has the requested field name.</param>
        /// <param name="propertyOrFieldName">Item field name.</param>
        /// <returns>Returns the requested field if it exists.</returns>
        private static Expression GetSafePropertyOrField( Expression item, string propertyOrFieldName )
        {
            if ( string.IsNullOrEmpty( propertyOrFieldName ) )
                throw new ArgumentException( $"{nameof( propertyOrFieldName )} is not specified." );

            var parts = propertyOrFieldName.Split( new char[] { '.' }, 2 );

            Expression field = null;

            MemberInfo memberInfo = GetSafeMember( item.Type, parts[0] );

            if ( memberInfo is PropertyInfo propertyInfo )
                field = Expression.Property( item, propertyInfo );
            else if ( memberInfo is FieldInfo fieldInfo )
                field = Expression.Field( item, fieldInfo );

            if ( field == null )
                throw new ArgumentException( $"Cannot detect the member of {item.Type}", propertyOrFieldName );

            if ( parts.Length > 1 )
                field = GetSafePropertyOrField( field, parts[1] );

            // if the value type cannot be null there's no reason to check it for null
            if ( !IsNullable( field.Type ) )
                return field;

            // check if field is null
            return Expression.Condition( Expression.Equal( item, Expression.Constant( null ) ),
                Expression.Constant( null, field.Type ),
                field );
        }

        // inspired by: https://stackoverflow.com/questions/2496256/expression-tree-with-property-inheritance-causes-an-argument-exception
        private static MemberInfo GetSafeMember( Type type, string fieldName )
        {
            MemberInfo memberInfo = (MemberInfo)type.GetProperty( fieldName )
                ?? type.GetField( fieldName );

            if ( memberInfo == null )
            {
                var baseTypesAndInterfaces = new List<Type>();

                if ( type.BaseType != null )
                {
                    baseTypesAndInterfaces.Add( type.BaseType );
                }

                baseTypesAndInterfaces.AddRange( type.GetInterfaces() );

                foreach ( var baseType in baseTypesAndInterfaces )
                {
                    memberInfo = GetSafeMember( baseType, fieldName );

                    if ( memberInfo != null )
                        break;
                }
            }

            return memberInfo;
        }


        /// <summary>
        /// Checks if requested type can bu nullable.
        /// </summary>
        /// <param name="type">Object type.</param>
        /// <returns></returns>
        private static bool IsNullable( Type type )
        {
            if ( type.IsClass )
                return true;

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof( Nullable<> );
        }

        /// <summary>
        /// Bulds an access expression for nested properties or fields.
        /// </summary>
        /// <param name="item">Item that has the requested field name.</param>
        /// <param name="propertyOrFieldName">Item field name.</param>
        /// <returns>Returns the requested field if it exists.</returns>
        private static Expression GetField( Expression item, string propertyOrFieldName )
        {
            if ( string.IsNullOrEmpty( propertyOrFieldName ) )
                throw new ArgumentException( $"{nameof( propertyOrFieldName )} is not specified." );

            var parts = propertyOrFieldName.Split( new char[] { '.' }, 2 );

            Expression subPropertyOrField = null;

            MemberInfo memberInfo = GetSafeMember( item.Type, propertyOrFieldName );

            if ( memberInfo is PropertyInfo propertyInfo )
                subPropertyOrField = Expression.Property( item, propertyInfo );
            else if ( memberInfo is FieldInfo fieldInfo )
                subPropertyOrField = Expression.Field( item, fieldInfo );

            if ( subPropertyOrField == null )
                throw new ArgumentException( $"Cannot detect the member of {item.Type}", propertyOrFieldName );

            if ( parts.Length > 1 )
                subPropertyOrField = GetField( subPropertyOrField, parts[1] );

            return subPropertyOrField;
        }

        public static Func<TItem, object> CreateValueGetter<TItem>( string fieldName )
        {
            var item = Expression.Parameter( typeof( TItem ), "item" );
            var property = GetSafePropertyOrField( item, fieldName );
            return Expression.Lambda<Func<TItem, object>>( Expression.Convert( property, typeof( object ) ), item ).Compile();
        }

        public static Func<Type> CreateValueTypeGetter<TItem>( string fieldName )
        {
            var item = Expression.Parameter( typeof( TItem ) );
            var property = GetField( item, fieldName );
            return Expression.Lambda<Func<Type>>( Expression.Constant( property.Type ) ).Compile();
        }

        public static Func<object> CreateDefaultValueByType<TItem>( string fieldName )
        {
            var item = Expression.Parameter( typeof( TItem ) );
            var property = GetField( item, fieldName );
            return Expression.Lambda<Func<object>>( Expression.Convert( Expression.Default( property.Type ), typeof( object ) ) ).Compile();
        }

        public static Action<TItem, object> CreateValueSetter<TItem>( string fieldName )
        {
            var item = Expression.Parameter( typeof( TItem ), "item" );
            var value = Expression.Parameter( typeof( object ), "value" );

            // There's ne safe field setter because that should be a developer responsibility
            // to don't allow for null nested fields. 
            var field = GetField( item, fieldName );
            return Expression.Lambda<Action<TItem, object>>( Expression.Assign( field, Expression.Convert( value, field.Type ) ), item, value ).Compile();
        }
    }
}
