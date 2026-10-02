#region Using directives
using System;
using System.Linq.Expressions;
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
        => MemberExpressionCompiler.CreateValueGetterExpression<TItem>( fieldName );

    /// <summary>
    /// Creates a null-safe member expression supporting nested paths.
    /// </summary>
    public static Expression GetSafePropertyOrFieldExpression( Expression item, string propertyOrFieldName )
        => MemberExpressionCompiler.GetSafePropertyOrFieldExpression( item, propertyOrFieldName );
}