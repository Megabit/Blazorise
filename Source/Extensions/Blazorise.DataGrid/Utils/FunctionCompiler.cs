using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

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
        /// <param name="fieldName">Item field name.</param>
        /// <returns>Returns the requested field if it exists.</returns>
        private static Expression GetSafeField( Expression item, string fieldName )
        {
            if ( string.IsNullOrEmpty( fieldName ) )
                throw new ArgumentException( $"{nameof( fieldName )} is not specified." );

            var parts = fieldName.Split( new char[] { '.' }, 2 );

            Expression field = Expression.PropertyOrField( item, parts[0] );

            if ( parts.Length > 1 )
                field = GetSafeField( field, parts[1] );

            // if the value type cannot be null there's no reason to check it for null
            if ( !IsNullable( field.Type ) )
                return field;

            // check if field is null
            return Expression.Condition( Expression.Equal( item, Expression.Constant( null ) ),
                Expression.Constant( null, field.Type ),
                field );
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
        /// <param name="fieldName">Item field name.</param>
        /// <returns>Returns the requested field if it exists.</returns>
        private static Expression GetField( Expression item, string fieldName )
        {
            if ( string.IsNullOrEmpty( fieldName ) )
                throw new ArgumentException( $"{nameof( fieldName )} is not specified." );

            var parts = fieldName.Split( new char[] { '.' }, 2 );

            Expression subProperty = Expression.PropertyOrField( item, parts[0] );

            if ( parts.Length > 1 )
                subProperty = GetField( subProperty, parts[1] );

            return subProperty;
        }

        public static Func<TItem, object> CreateValueGetter<TItem>( string fieldName )
        {
            var item = Expression.Parameter( typeof( TItem ), "item" );
            var property = GetSafeField( item, fieldName );
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
