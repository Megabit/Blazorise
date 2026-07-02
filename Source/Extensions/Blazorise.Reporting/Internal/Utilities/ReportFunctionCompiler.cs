#region Using directives
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportFunctionCompiler
{
    #region Members

    private static readonly ConcurrentDictionary<ReportValueGetterKey, Func<object, object>> ValueGetters = new();

    private static readonly Func<object, object> NullValueGetter = _ => null;

    #endregion

    #region Methods

    internal static object GetValue( object item, string fieldName )
    {
        if ( item is null || string.IsNullOrWhiteSpace( fieldName ) )
            return null;

        var getter = ValueGetters.GetOrAdd( new( item.GetType(), fieldName ), CreateValueGetter );

        return getter.Invoke( item );
    }

    internal static bool TryGetValue( object item, string fieldName, out object value )
    {
        value = null;

        if ( item is null || string.IsNullOrWhiteSpace( fieldName ) )
            return false;

        if ( GetSafeMember( item.GetType(), fieldName ) is null )
            return false;

        value = GetValue( item, fieldName );

        return true;
    }

    private static Func<object, object> CreateValueGetter( ReportValueGetterKey key )
    {
        var member = GetSafeMember( key.ItemType, key.FieldName );

        if ( member is null )
            return NullValueGetter;

        try
        {
            var item = Expression.Parameter( typeof( object ), "item" );
            var typedItem = Expression.Convert( item, key.ItemType );
            var memberExpression = CreateMemberExpression( typedItem, member );
            var value = Expression.Convert( memberExpression, typeof( object ) );
            var body = Expression.Condition(
                Expression.Equal( item, Expression.Constant( null ) ),
                Expression.Constant( null, typeof( object ) ),
                value );

            return Expression.Lambda<Func<object, object>>( body, item ).Compile();
        }
        catch
        {
            return item => GetValueByReflection( item, key.FieldName );
        }
    }

    private static Expression CreateMemberExpression( Expression item, MemberInfo member )
    {
        return member switch
        {
            PropertyInfo propertyInfo => Expression.Property( item, propertyInfo ),
            FieldInfo fieldInfo => Expression.Field( item, fieldInfo ),
            _ => null,
        };
    }

    private static object GetValueByReflection( object item, string fieldName )
    {
        if ( item is null || string.IsNullOrWhiteSpace( fieldName ) )
            return null;

        return GetSafeMember( item.GetType(), fieldName ) switch
        {
            PropertyInfo propertyInfo => propertyInfo.GetValue( item ),
            FieldInfo fieldInfo => fieldInfo.GetValue( item ),
            _ => null,
        };
    }

    private static MemberInfo GetSafeMember( Type type, string fieldName )
    {
        if ( type is null || string.IsNullOrWhiteSpace( fieldName ) )
            return null;

        var memberInfo = FindProperty( type, fieldName ) as MemberInfo
            ?? FindField( type, fieldName );

        if ( memberInfo is not null )
            return memberInfo;

        if ( type.BaseType is not null )
        {
            memberInfo = GetSafeMember( type.BaseType, fieldName );

            if ( memberInfo is not null )
                return memberInfo;
        }

        foreach ( var interfaceType in type.GetInterfaces() )
        {
            memberInfo = GetSafeMember( interfaceType, fieldName );

            if ( memberInfo is not null )
                return memberInfo;
        }

        return null;
    }

    private static PropertyInfo FindProperty( Type type, string fieldName )
    {
        return type.GetProperties( BindingFlags.Instance | BindingFlags.Public )
            .FirstOrDefault( property => string.Equals( property.Name, fieldName, StringComparison.OrdinalIgnoreCase ) );
    }

    private static FieldInfo FindField( Type type, string fieldName )
    {
        return type.GetFields( BindingFlags.Instance | BindingFlags.Public )
            .FirstOrDefault( field => string.Equals( field.Name, fieldName, StringComparison.OrdinalIgnoreCase ) );
    }

    #endregion

    #region Structs

    private readonly record struct ReportValueGetterKey( Type ItemType, string FieldName );

    #endregion
}