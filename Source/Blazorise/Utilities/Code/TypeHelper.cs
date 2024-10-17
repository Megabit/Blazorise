#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Helper methods used to determine types.
/// </summary>
public static class TypeHelper
{
    private static readonly HashSet<Type> BooleanTypes = new HashSet<Type>
    {
        typeof(bool),
        typeof(bool?)
    };

    private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
    {
        typeof(int),
        typeof(double),
        typeof(decimal),
        typeof(long),
        typeof(short),
        typeof(sbyte),
        typeof(byte),
        typeof(ulong),
        typeof(ushort),
        typeof(uint),
        typeof(float)
    };

    private static readonly HashSet<Type> IntegerTypes = new HashSet<Type>
    {
        typeof(int),
        typeof(long),
        typeof(sbyte),
        typeof(byte),
        typeof(ulong),
        typeof(ushort),
        typeof(uint),
    };

    private static readonly HashSet<Type> DateTypes = new HashSet<Type>
    {
        typeof(DateTime),
        typeof(DateTime?),
        typeof(DateOnly),
        typeof(DateOnly?),
        typeof(DateTimeOffset),
        typeof(DateTimeOffset?),
    };

    private static readonly HashSet<Type> TimeTypes = new HashSet<Type>
    {
        typeof(TimeOnly),
        typeof(TimeOnly?),
        typeof(TimeSpan),
        typeof(TimeSpan?),
    };

    /// <summary>
    /// Determines if the supplied type is a number.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns>True if the type is a number.</returns>
    public static bool IsNumeric( this Type type )
    {
        return NumericTypes.Contains( Nullable.GetUnderlyingType( type ) ?? type );
    }

    /// <summary>
    /// Determines if the supplied type is a bool.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns>True if the type is a boolean.</returns>
    public static bool IsBoolean( this Type type )
    {
        return BooleanTypes.Contains( Nullable.GetUnderlyingType( type ) ?? type );
    }

    /// <summary>
    /// Determines if the supplied type is an integer.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns>True if the type is an integer.</returns>
    public static bool IsInteger( this Type type )
    {
        return IntegerTypes.Contains( Nullable.GetUnderlyingType( type ) ?? type );
    }

    /// <summary>
    /// Determines if the supplied type is a date.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns>True if the type is a date.</returns>
    public static bool IsDate( this Type type )
    {
        return DateTypes.Contains( Nullable.GetUnderlyingType( type ) ?? type );
    }

    /// <summary>
    /// Determines if the supplied type is a time.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns>True if the type is a time.</returns>
    public static bool IsTime( this Type type )
    {
        return TimeTypes.Contains( Nullable.GetUnderlyingType( type ) ?? type );
    }
}