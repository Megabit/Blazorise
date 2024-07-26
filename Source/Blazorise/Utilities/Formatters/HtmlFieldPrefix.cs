#region Using directives
using System;
using System.Linq.Expressions;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Represents a prefix for HTML field names.
/// </summary>
/// <remarks>
/// Copy from https://github.com/dotnet/aspnetcore/blob/ee4a0e9801c4e1e673d0373e1c51a6790de174cc/src/Components/Web/src/Forms/HtmlFieldPrefix.cs
/// </remarks>
public class HtmlFieldPrefix
{
    #region Members

    private readonly LambdaExpression[] rest = Array.Empty<LambdaExpression>();

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlFieldPrefix"/> class with the initial lambda expression.
    /// </summary>
    /// <param name="initial">The initial lambda expression.</param>
    public HtmlFieldPrefix( LambdaExpression initial )
    {
        Initial = initial;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlFieldPrefix"/> class with the specified lambda expressions.
    /// </summary>
    /// <param name="expression">The initial lambda expression.</param>
    /// <param name="rest">The additional lambda expressions.</param>
    public HtmlFieldPrefix( LambdaExpression expression, params LambdaExpression[] rest )
        : this( expression )
    {
        this.rest = rest;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Combines the current HTML field prefix with another lambda expression.
    /// </summary>
    /// <param name="other">The lambda expression to combine with.</param>
    /// <returns>A new <see cref="HtmlFieldPrefix"/> that includes the combined lambda expressions.</returns>
    public HtmlFieldPrefix Combine( LambdaExpression other )
    {
        var restLength = rest?.Length ?? 0;
        var length = restLength + 1;
        var expressions = new LambdaExpression[length];
        for ( var i = 0; i < restLength - 1; i++ )
        {
            expressions[i] = rest![i];
        }

        expressions[length - 1] = other;

        return new HtmlFieldPrefix( Initial, expressions );
    }

    /// <summary>
    /// Gets the field name for the given lambda expression.
    /// </summary>
    /// <param name="expression">The lambda expression to get the field name for.</param>
    /// <returns>The field name as a string.</returns>
    public string GetFieldName( LambdaExpression expression )
    {
        var prefix = ExpressionFormatter.FormatLambda( Initial );
        var restLength = rest?.Length ?? 0;
        for ( var i = 0; i < restLength; i++ )
        {
            prefix = ExpressionFormatter.FormatLambda( rest![i], prefix );
        }

        return ExpressionFormatter.FormatLambda( expression, prefix );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the initial lambda expression.
    /// </summary>
    public LambdaExpression Initial { get; }

    #endregion
}
