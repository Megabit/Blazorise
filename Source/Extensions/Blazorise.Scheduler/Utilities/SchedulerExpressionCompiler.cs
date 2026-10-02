#region Using directives
using System;
using System.Linq.Expressions;
#endregion

namespace Blazorise.Scheduler.Utilities;

/// <summary>
/// Utility class for dynamically compiling expressions to access properties or fields on scheduler item models.
/// </summary>
public static class SchedulerExpressionCompiler
{
    /// <summary>
    /// Builds a compiled getter function that returns a string value for a specified property.
    /// </summary>
    /// <typeparam name="TItem">The item type that contains the property.</typeparam>
    /// <param name="fieldName">The name of the property to read.</param>
    /// <returns>A compiled function that returns a string value, or <c>null</c> if the property is not found.</returns>
    public static Func<TItem, string> BuildGetStringFunc<TItem>( string fieldName )
    {
        var itemType = typeof( TItem );
        var itemParameter = Expression.Parameter( itemType, "x" );

        var property = itemType.GetProperty( fieldName );

        if ( property is null )
        {
            return null;
        }

        var titlePropertyAccess = Expression.Property( itemParameter, property );

        var lambda = Expression.Lambda<Func<TItem, string>>( titlePropertyAccess, itemParameter );

        return lambda.Compile();
    }

    /// <summary>
    /// Builds a compiled getter function that returns an object value for a specified property.
    /// </summary>
    /// <typeparam name="TItem">The item type that contains the property.</typeparam>
    /// <param name="fieldName">The name of the property to read.</param>
    /// <returns>A compiled function that returns the property value as an object, or <c>null</c> if not found.</returns>
    public static Func<TItem, object> BuildGetValueFunc<TItem>( string fieldName )
    {
        var itemType = typeof( TItem );
        var itemParameter = Expression.Parameter( itemType, "x" );

        var property = itemType.GetProperty( fieldName );

        if ( property is null )
        {
            return null;
        }

        var propertyAccess = Expression.Property( itemParameter, property );
        var convert = Expression.Convert( propertyAccess, typeof( object ) );

        var lambda = Expression.Lambda<Func<TItem, object>>( convert, itemParameter );

        return lambda.Compile();
    }

    /// <summary>
    /// Creates a getter expression for a property or field, returning an object.
    /// </summary>
    /// <typeparam name="TItem">The item type that contains the member.</typeparam>
    /// <param name="fieldName">The property or field name, supporting nested paths with dot notation.</param>
    /// <returns>A LINQ expression that returns an object value.</returns>
    public static Expression<Func<TItem, object>> CreateValueGetterExpression<TItem>( string fieldName )
        => MemberExpressionCompiler.CreateValueGetterExpression<TItem>( fieldName );

    /// <summary>
    /// Creates a strongly typed getter expression for a property or field.
    /// </summary>
    /// <typeparam name="TItem">The item type that contains the member.</typeparam>
    /// <typeparam name="TValue">The expected return type of the property.</typeparam>
    /// <param name="fieldName">The property or field name, supporting nested paths with dot notation.</param>
    /// <returns>A LINQ expression that returns a typed value.</returns>
    public static Expression<Func<TItem, TValue>> CreateValueGetterExpression<TItem, TValue>( string fieldName )
        => MemberExpressionCompiler.CreateValueGetterExpression<TItem, TValue>( fieldName );

    /// <summary>
    /// Builds a null-safe expression for accessing a property or field, optionally supporting nested paths (e.g. "Address.Street").
    /// </summary>
    /// <param name="item">The root expression (e.g., the item parameter).</param>
    /// <param name="propertyOrFieldName">The name of the property or field, optionally including dot notation.</param>
    /// <returns>The expression for the accessed member.</returns>
    public static Expression GetSafePropertyOrFieldExpression( Expression item, string propertyOrFieldName )
        => MemberExpressionCompiler.GetSafePropertyOrFieldExpression( item, propertyOrFieldName );

    /// <summary>
    /// Builds a non-null-safe member expression for accessing nested fields or properties.
    /// </summary>
    /// <param name="item">The root expression (e.g., parameter of the lambda).</param>
    /// <param name="propertyOrFieldName">The dot-separated path to the property or field.</param>
    /// <returns>A member access expression.</returns>
    public static MemberExpression GetPropertyOrFieldExpression( Expression item, string propertyOrFieldName )
        => MemberExpressionCompiler.GetPropertyOrFieldExpression( item, propertyOrFieldName );
}