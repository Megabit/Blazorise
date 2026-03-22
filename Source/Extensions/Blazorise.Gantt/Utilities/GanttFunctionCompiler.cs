#region Using directives
using System;
using System.Linq.Expressions;
#endregion

namespace Blazorise.Gantt.Utilities;

/// <summary>
/// Utility class for compiling field access delegates.
/// </summary>
public static class GanttFunctionCompiler
{
    /// <summary>
    /// Creates a default item constructor delegate.
    /// </summary>
    public static Func<TItem> CreateNewItem<TItem>()
    {
        return Expression.Lambda<Func<TItem>>( Expression.New( typeof( TItem ) ) ).Compile();
    }

    /// <summary>
    /// Creates a dynamic getter.
    /// </summary>
    public static Func<TItem, object> CreateValueGetter<TItem>( string fieldName )
    {
        return GanttExpressionCompiler.CreateValueGetterExpression<TItem>( fieldName ).Compile();
    }

    /// <summary>
    /// Creates a typed getter.
    /// </summary>
    public static Func<TItem, TValue> CreateValueGetter<TItem, TValue>( string fieldName )
    {
        return GanttExpressionCompiler.CreateValueGetterExpression<TItem, TValue>( fieldName ).Compile();
    }

    /// <summary>
    /// Creates a dynamic setter.
    /// </summary>
    public static Action<TItem, object> CreateValueSetter<TItem>( string fieldName )
    {
        var item = Expression.Parameter( typeof( TItem ), "item" );
        var value = Expression.Parameter( typeof( object ), "value" );

        var field = GanttExpressionCompiler.GetPropertyOrFieldExpression( item, fieldName );

        return Expression.Lambda<Action<TItem, object>>( Expression.Assign( field, Expression.Convert( value, field.Type ) ), item, value ).Compile();
    }

    /// <summary>
    /// Creates a typed setter.
    /// </summary>
    public static Action<TItem, TValue> CreateValueSetter<TItem, TValue>( string fieldName )
    {
        var item = Expression.Parameter( typeof( TItem ), "item" );
        var value = Expression.Parameter( typeof( TValue ), "value" );

        var field = GanttExpressionCompiler.GetPropertyOrFieldExpression( item, fieldName );

        return Expression.Lambda<Action<TItem, TValue>>( Expression.Assign( field, Expression.Convert( value, field.Type ) ), item, value ).Compile();
    }
}