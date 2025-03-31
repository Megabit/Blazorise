#region Using directives
using System;
using System.Linq.Expressions;
#endregion

namespace Blazorise.Scheduler.Utilities;

/// <summary>
/// Utility class for compiling expressions.
/// </summary>
public static class SchedulerFunctionCompiler
{
    public static Func<TItem> CreateNewItem<TItem>()
    {
        return Expression.Lambda<Func<TItem>>( Expression.New( typeof( TItem ) ) ).Compile();
    }

    public static Func<TItem, object> CreateValueGetter<TItem>( string fieldName )
    {
        return SchedulerExpressionCompiler.CreateValueGetterExpression<TItem>( fieldName ).Compile();
    }

    public static Func<TItem, TValue> CreateValueGetter<TItem, TValue>( string fieldName )
    {
        return SchedulerExpressionCompiler.CreateValueGetterExpression<TItem, TValue>( fieldName ).Compile();
    }

    public static Action<TItem, object> CreateValueSetter<TItem>( string fieldName )
    {
        var item = Expression.Parameter( typeof( TItem ), "item" );
        var value = Expression.Parameter( typeof( object ), "value" );

        var field = SchedulerExpressionCompiler.GetPropertyOrFieldExpression( item, fieldName );
        return Expression.Lambda<Action<TItem, object>>( Expression.Assign( field, Expression.Convert( value, field.Type ) ), item, value ).Compile();
    }

    public static Action<TItem, TValue> CreateValueSetter<TItem, TValue>( string fieldName )
    {
        var item = Expression.Parameter( typeof( TItem ), "item" );
        var value = Expression.Parameter( typeof( TValue ), "value" );

        var field = SchedulerExpressionCompiler.GetPropertyOrFieldExpression( item, fieldName );
        return Expression.Lambda<Action<TItem, TValue>>( Expression.Assign( field, Expression.Convert( value, field.Type ) ), item, value ).Compile();
    }
}
