#region Using directives
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Scriptable options also accept a function which is called for each of the underlying data values and that takes the unique
/// argument context representing contextual information (see <see cref="ScriptableOptionsContext"/>).
/// </summary>
/// <typeparam name="TOptions">A value that is returned from a function.</typeparam>
/// <typeparam name="TValue">The current label value of the dataset</typeparam>
/// <typeparam name="TContext">A context representing contextual information.</typeparam>
public class ScriptableValueBasedOptions<TOptions, TValue, TContext>
    : IEquatable<ScriptableValueBasedOptions<TOptions, TValue, TContext>>
    where TContext : ScriptableOptionsContext 
{
    #region Members

    private readonly TOptions value;

    private readonly Expression<Func<TValue, TContext, TOptions>> scriptableValue;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new instance of <see cref="ScriptableValueBasedOptions{TOptions, TValue, TContext}"/> which represents a single value.
    /// </summary>
    /// <param name="value">The single value this <see cref="ScriptableValueBasedOptions{TOptions, TValue, TContext}"/> should represent.</param>
    public ScriptableValueBasedOptions( TOptions value )
    {
        this.value = value;

        IsScriptable = false;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ScriptableValueBasedOptions{TOptions, TValue, TContext}"/> which represents a scriptable value.
    /// </summary>
    /// <param name="scriptableValue">The scriptable value this <see cref="ScriptableValueBasedOptions{TOptions, TValue, TContext}"/> should represent.</param>
    public ScriptableValueBasedOptions( Expression<Func<TValue, TContext, TOptions>> scriptableValue )
    {
        this.scriptableValue = scriptableValue;

        IsScriptable = true;
    }

    #endregion

    #region Operators

    /// <summary>
    /// Implicitly wraps a single value of <typeparamref name="TOptions"/> to a new instance of <see cref="ScriptableValueBasedOptions{TOptions, TValue, TContext}"/>.
    /// </summary>
    /// <param name="value">The single value to wrap.</param>
    //public static implicit operator ScriptableValueBasedOptions<TOptions, TValue, TContext>( TOptions value )
    //{
    //    return new ScriptableValueBasedOptions<TOptions, TValue, TContext>( value );
    //}

    /// <summary>
    /// Implicitly wraps an expression of <typeparamref name="TOptions"/> to a new instance of <see cref="ScriptableValueBasedOptions{TOptions, TValue, TContext}"/>.
    /// </summary>
    /// <param name="scriptableValue">The expression values to wrap.</param>
    public static implicit operator ScriptableValueBasedOptions<TOptions, TValue, TContext>( Expression<Func<TValue, TContext, TOptions>> scriptableValue )
    {
        return new ScriptableValueBasedOptions<TOptions, TValue, TContext>( scriptableValue );
    }

    /// <summary>
    /// Determines whether two specified <see cref="ScriptableValueBasedOptions{TOptions, TValue, TContext}"/> instances contain the same value.
    /// </summary>
    /// <param name="a">The first <see cref="ScriptableValueBasedOptions{TOptions, TValue, TContext}"/> to compare</param>
    /// <param name="b">The second <see cref="ScriptableValueBasedOptions{TOptions, TValue, TContext}"/> to compare</param>
    /// <returns>true if the value of a is the same as the value of b; otherwise, false.</returns>
    public static bool operator ==( ScriptableValueBasedOptions<TOptions, TValue, TContext> a, ScriptableValueBasedOptions<TOptions, TValue, TContext> b ) => a.Equals( b );

    /// <summary>
    /// Determines whether two specified <see cref="ScriptableValueBasedOptions{TOptions, TValue, TContext}"/> instances contain different values.
    /// </summary>
    /// <param name="a">The first <see cref="ScriptableValueBasedOptions{TOptions, TValue, TContext}"/> to compare</param>
    /// <param name="b">The second <see cref="ScriptableValueBasedOptions{TOptions, TValue, TContext}"/> to compare</param>
    /// <returns>true if the value of a is different from the value of b; otherwise, false.</returns>
    public static bool operator !=( ScriptableValueBasedOptions<TOptions, TValue, TContext> a, ScriptableValueBasedOptions<TOptions, TValue, TContext> b ) => !( a == b );

    #endregion

    #region Methods

    /// <summary>
    /// Determines whether the specified <see cref="ScriptableValueBasedOptions{TOptions, TValue, TContext}"/> instance is considered equal to the current instance.
    /// </summary>
    /// <param name="other">The <see cref="ScriptableValueBasedOptions{TOptions, TValue, TContext}"/> to compare with.</param>
    /// <returns>true if the objects are considered equal; otherwise, false.</returns>
    public bool Equals( ScriptableValueBasedOptions<TOptions, TValue, TContext> other )
    {
        if ( IsScriptable != other.IsScriptable )
            return false;

        if ( IsScriptable )
        {
            return ScriptableValue == other.ScriptableValue;
        }
        else
        {
            return EqualityComparer<TOptions>.Default.Equals( Value, other.Value );
        }
    }

    /// <summary>
    /// Determines whether the specified object instance is considered equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>true if the objects are considered equal; otherwise, false.</returns>
    public override bool Equals( object obj )
    {
        if ( obj == null )
            return false;

        if ( obj is ScriptableValueBasedOptions<TOptions, TValue, TContext> option )
        {
            return Equals( option );
        }
        else
        {
            if ( IsScriptable )
            {
                return ScriptableValue.Equals( obj );
            }
            else
            {
                return Value.Equals( obj );
            }
        }
    }

    /// <summary>
    /// Returns the hash of the underlying object.
    /// </summary>
    /// <returns>The hash of the underlying object.</returns>
    public override int GetHashCode()
    {
        var hashCode = -506568782;
        hashCode = hashCode * -1521134295 + EqualityComparer<TOptions>.Default.GetHashCode( Value );
        hashCode = hashCode * -1521134295 + EqualityComparer<Expression<Func<TValue, TContext, TOptions>>>.Default.GetHashCode( ScriptableValue );
        hashCode = hashCode * -1521134295 + IsScriptable.GetHashCode();
        return hashCode;
    }

    #endregion

    #region Properties

    // for serialization, there has to be a cast to object anyway
    internal object BoxedValue => IsScriptable ? ScriptableValue : Value;

    /// <summary>
    /// Gets the value indicating whether the option wrapped in this <see cref="ScriptableValueBasedOptions{TOptions, TValue, TContext}"/> is scriptable.
    /// </summary>
    public bool IsScriptable { get; }

    /// <summary>
    /// The single value represented by this instance.
    /// </summary>
    public TOptions Value
    {
        get
        {
            if ( IsScriptable )
                throw new InvalidOperationException( "This instance represents an scriptable values. The scriptable values is not available." );

            return value;
        }
    }

    /// <summary>
    /// The scriptable value represented by this instance.
    /// </summary>
    public Expression<Func<TValue, TContext, TOptions>> ScriptableValue
    {
        get
        {
            if ( !IsScriptable )
                throw new InvalidOperationException( "This instance represents a single value. The scriptable values is not available." );

            return scriptableValue;
        }
    }

    #endregion
}