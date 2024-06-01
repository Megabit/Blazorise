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
    /// Determines if the supplied type is an integer.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns>True if the type is an integer.</returns>
    public static bool IsInteger( this Type type )
    {
        return IntegerTypes.Contains( Nullable.GetUnderlyingType( type ) ?? type );
    }
}