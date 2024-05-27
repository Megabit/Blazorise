#region Using directives
using System;
using System.Linq.Expressions;
#endregion

namespace Blazorise;

/// <summary>
/// Helper for various expression based methods.
/// </summary>
public static class ExpressionCompiler
{
    /// <summary>
    /// Gets a property in an unknown instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static T GetProperty<T>( object instance, string propertyName )
        => CreatePropertyGetter<T>( instance, propertyName )( instance );

    /// <summary>
    /// Generates a function getter for a property in an unknown instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static Func<object, T> CreatePropertyGetter<T>( object instance, string propertyName )
            => CreatePropertyGetter<T>( instance.GetType(), propertyName );

    /// <summary>
    /// Generates a function getter for a property in an unknown instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instanceType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static Func<object, T> CreatePropertyGetter<T>( Type instanceType, string propertyName )
    {
        var parameterExp = Expression.Parameter( typeof( object ), "instance" );
        var castExp = Expression.TypeAs( parameterExp, instanceType );
        var property = Expression.Property( castExp, propertyName );

        return Expression.Lambda<Func<object, T>>( Expression.Convert( property, typeof( T ) ), parameterExp ).Compile();
    }

    /// <summary>
    /// Gets a field in an unknown instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static T GetField<T>( object instance, string fieldName )
        => CreateFieldGetter<T>( instance, fieldName )( instance );

    /// <summary>
    /// Generates a function getter for a field in an unknown instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static Func<object, T> CreateFieldGetter<T>( object instance, string fieldName )
        => CreateFieldGetter<T>( instance.GetType(), fieldName );

    /// <summary>
    /// Generates a function getter for a field in an unknown instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instanceType"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static Func<object, T> CreateFieldGetter<T>( Type instanceType, string fieldName )
    {
        var parameterExp = Expression.Parameter( typeof( object ), "instance" );
        var castExp = Expression.TypeAs( parameterExp, instanceType );
        var field = Expression.Field( castExp, fieldName );

        return Expression.Lambda<Func<object, T>>( Expression.Convert( field, typeof( T ) ), parameterExp ).Compile();
    }
}