#region Using directives
using System;
using System.Linq.Expressions;
#endregion

namespace Blazorise.Gantt.Utilities;

/// <summary>
/// Utility class for generating safe member access expressions.
/// </summary>
public static class GanttExpressionCompiler
{
    /// <summary>
    /// Creates an object getter expression.
    /// </summary>
    public static Expression<Func<TItem, object>> CreateValueGetterExpression<TItem>( string fieldName )
        => MemberExpressionCompiler.CreateValueGetterExpression<TItem>( fieldName );

    /// <summary>
    /// Creates a typed getter expression.
    /// </summary>
    public static Expression<Func<TItem, TValue>> CreateValueGetterExpression<TItem, TValue>( string fieldName )
        => MemberExpressionCompiler.CreateValueGetterExpression<TItem, TValue>( fieldName );

    /// <summary>
    /// Creates a null-safe member expression supporting nested paths.
    /// </summary>
    public static Expression GetSafePropertyOrFieldExpression( Expression item, string propertyOrFieldName )
        => MemberExpressionCompiler.GetSafePropertyOrFieldExpression( item, propertyOrFieldName );

    /// <summary>
    /// Creates a direct member expression supporting nested paths.
    /// </summary>
    public static MemberExpression GetPropertyOrFieldExpression( Expression item, string propertyOrFieldName )
        => MemberExpressionCompiler.GetPropertyOrFieldExpression( item, propertyOrFieldName );
}