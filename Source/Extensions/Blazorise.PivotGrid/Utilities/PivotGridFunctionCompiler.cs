#region Using directives
using System;
#endregion

namespace Blazorise.PivotGrid.Utilities;

/// <summary>
/// Utility class for compiling field access delegates.
/// </summary>
public static class PivotGridFunctionCompiler
{
    /// <summary>
    /// Creates a dynamic getter.
    /// </summary>
    public static Func<TItem, object> CreateValueGetter<TItem>( string fieldName )
    {
        return PivotGridExpressionCompiler.CreateValueGetterExpression<TItem>( fieldName ).Compile();
    }
}