#region Using directives
using System;
using System.Linq.Expressions;
#endregion

namespace Blazorise.Scheduler.Utilities;

/// <summary>
/// Utility class for dynamically compiling property accessors and mutators for scheduler item types.
/// </summary>
public static class SchedulerFunctionCompiler
{
    /// <summary>
    /// Creates a new instance constructor delegate for the specified item type.
    /// </summary>
    /// <typeparam name="TItem">The type of the item to instantiate.</typeparam>
    /// <returns>A compiled delegate that returns a new instance of <typeparamref name="TItem"/>.</returns>
    public static Func<TItem> CreateNewItem<TItem>()
    {
        return Expression.Lambda<Func<TItem>>( Expression.New( typeof( TItem ) ) ).Compile();
    }

    /// <summary>
    /// Creates a dynamic getter delegate that returns an object from the specified property or field.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="fieldName">The name of the property or field.</param>
    /// <returns>A compiled delegate that returns the value as <see cref="object"/>.</returns>
    public static Func<TItem, object> CreateValueGetter<TItem>( string fieldName )
    {
        return SchedulerExpressionCompiler.CreateValueGetterExpression<TItem>( fieldName ).Compile();
    }

    /// <summary>
    /// Creates a strongly typed dynamic getter delegate for the specified property or field.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TValue">The expected return type of the property or field.</typeparam>
    /// <param name="fieldName">The name of the property or field.</param>
    /// <returns>A compiled delegate that returns the value as <typeparamref name="TValue"/>.</returns>
    public static Func<TItem, TValue> CreateValueGetter<TItem, TValue>( string fieldName )
    {
        return SchedulerExpressionCompiler.CreateValueGetterExpression<TItem, TValue>( fieldName ).Compile();
    }

    /// <summary>
    /// Creates a dynamic setter delegate that assigns a value to the specified property or field using an <see cref="object"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="fieldName">The name of the property or field to assign to.</param>
    /// <returns>A compiled delegate that sets the property value.</returns>
    public static Action<TItem, object> CreateValueSetter<TItem>( string fieldName )
    {
        var item = Expression.Parameter( typeof( TItem ), "item" );
        var value = Expression.Parameter( typeof( object ), "value" );

        var field = SchedulerExpressionCompiler.GetPropertyOrFieldExpression( item, fieldName );
        return Expression.Lambda<Action<TItem, object>>( Expression.Assign( field, Expression.Convert( value, field.Type ) ), item, value ).Compile();
    }

    /// <summary>
    /// Creates a strongly typed dynamic setter delegate for the specified property or field.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TValue">The value type to assign.</typeparam>
    /// <param name="fieldName">The name of the property or field to assign to.</param>
    /// <returns>A compiled delegate that sets the property value using <typeparamref name="TValue"/>.</returns>
    public static Action<TItem, TValue> CreateValueSetter<TItem, TValue>( string fieldName )
    {
        var item = Expression.Parameter( typeof( TItem ), "item" );
        var value = Expression.Parameter( typeof( TValue ), "value" );

        var field = SchedulerExpressionCompiler.GetPropertyOrFieldExpression( item, fieldName );
        return Expression.Lambda<Action<TItem, TValue>>( Expression.Assign( field, Expression.Convert( value, field.Type ) ), item, value ).Compile();
    }
}
