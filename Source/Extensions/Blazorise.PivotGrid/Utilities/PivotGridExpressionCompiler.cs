#region Using directives
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
#endregion

namespace Blazorise.PivotGrid.Utilities;

/// <summary>
/// Utility class for generating null-safe member access expressions.
/// </summary>
public static class PivotGridExpressionCompiler
{
    /// <summary>
    /// Creates an object getter expression for a field path.
    /// </summary>
    public static Expression<Func<TItem, object>> CreateValueGetterExpression<TItem>( string fieldName )
    {
        var item = Expression.Parameter( typeof( TItem ), "item" );
        var property = GetSafePropertyOrFieldExpression( item, fieldName );

        return Expression.Lambda<Func<TItem, object>>( Expression.Convert( property, typeof( object ) ), item );
    }

    /// <summary>
    /// Creates a null-safe member expression supporting nested paths.
    /// </summary>
    public static Expression GetSafePropertyOrFieldExpression( Expression item, string propertyOrFieldName )
    {
        if ( string.IsNullOrEmpty( propertyOrFieldName ) )
            throw new ArgumentException( $"{nameof( propertyOrFieldName )} is not specified." );

        var parts = propertyOrFieldName.Split( new char[] { '.' }, 2 );
        Expression field = null;
        var memberInfo = GetSafeMember( item.Type, parts[0] );

        if ( memberInfo is PropertyInfo propertyInfo )
            field = Expression.Property( item, propertyInfo );
        else if ( memberInfo is FieldInfo fieldInfo )
            field = Expression.Field( item, fieldInfo );

        if ( field is null )
            throw new ArgumentException( $"Cannot detect the member of {item.Type}", propertyOrFieldName );

        field = Expression.Condition(
            Expression.Equal( item, Expression.Default( item.Type ) ),
            IsNullable( field.Type ) ? Expression.Constant( null, field.Type ) : Expression.Default( field.Type ),
            field );

        if ( parts.Length > 1 )
            field = GetSafePropertyOrFieldExpression( field, parts[1] );

        return field;
    }

    private static MemberInfo GetSafeMember( Type type, string fieldName )
    {
        MemberInfo memberInfo = (MemberInfo)type.GetProperty( fieldName ) ?? type.GetField( fieldName );

        if ( memberInfo is null )
        {
            var baseTypesAndInterfaces = new List<Type>();

            if ( type.BaseType is not null )
                baseTypesAndInterfaces.Add( type.BaseType );

            baseTypesAndInterfaces.AddRange( type.GetInterfaces() );

            foreach ( var baseType in baseTypesAndInterfaces )
            {
                memberInfo = GetSafeMember( baseType, fieldName );

                if ( memberInfo is not null )
                    break;
            }
        }

        return memberInfo;
    }

    private static bool IsNullable( Type type )
    {
        if ( type.IsClass )
            return true;

        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof( Nullable<> );
    }
}