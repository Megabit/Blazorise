#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Utility methods for working with responsive <see cref="Breakpoint"/> values.
/// </summary>
public static class BreakpointExtensions
{
    /// <summary>
    /// Parses a breakpoint name into its canonical <see cref="Breakpoint"/> value.
    /// </summary>
    /// <param name="breakpoint">Breakpoint name.</param>
    /// <returns>Parsed <see cref="Breakpoint"/>.</returns>
    public static Breakpoint ParseBreakpoint( this string breakpoint )
    {
        if ( string.IsNullOrWhiteSpace( breakpoint ) )
            return Breakpoint.None;

        string normalized = breakpoint
            .Trim()
            .Replace( "-", string.Empty, StringComparison.Ordinal )
            .Replace( "_", string.Empty, StringComparison.Ordinal )
            .ToLowerInvariant();

        return normalized switch
        {
            "mobile" or "xs" or "extrasmall" => Breakpoint.Mobile,
            "tablet" or "sm" or "small" => Breakpoint.Tablet,
            "desktop" or "md" or "medium" => Breakpoint.Desktop,
            "widescreen" or "lg" or "large" => Breakpoint.Widescreen,
            "fullhd" or "xl" or "extralarge" => Breakpoint.FullHD,
            "quadhd" or "xxl" or "extraextralarge" => Breakpoint.QuadHD,
            _ => Breakpoint.None,
        };
    }

    /// <summary>
    /// Normalizes aliased breakpoints to their canonical values.
    /// </summary>
    /// <param name="breakpoint">Breakpoint to normalize.</param>
    /// <returns>Canonical <see cref="Breakpoint"/> value.</returns>
    public static Breakpoint Normalize( this Breakpoint breakpoint )
    {
        return breakpoint switch
        {
            Breakpoint.ExtraSmall => Breakpoint.Mobile,
            Breakpoint.Small => Breakpoint.Tablet,
            Breakpoint.Medium => Breakpoint.Desktop,
            Breakpoint.Large => Breakpoint.Widescreen,
            Breakpoint.ExtraLarge => Breakpoint.FullHD,
            Breakpoint.ExtraExtraLarge => Breakpoint.QuadHD,
            _ => breakpoint,
        };
    }

    /// <summary>
    /// Determines if the current breakpoint is at least the requested breakpoint.
    /// </summary>
    /// <param name="breakpoint">Current breakpoint.</param>
    /// <param name="compareTo">Breakpoint to compare against.</param>
    /// <returns>True if the breakpoint is at least the requested value.</returns>
    public static bool IsAtLeast( this Breakpoint breakpoint, Breakpoint compareTo )
    {
        return GetOrder( breakpoint ) >= GetOrder( compareTo );
    }

    /// <summary>
    /// Determines if the current breakpoint is below the requested breakpoint.
    /// </summary>
    /// <param name="breakpoint">Current breakpoint.</param>
    /// <param name="compareTo">Breakpoint to compare against.</param>
    /// <returns>True if the breakpoint is below the requested value.</returns>
    public static bool IsBelow( this Breakpoint breakpoint, Breakpoint compareTo )
    {
        return GetOrder( breakpoint ) < GetOrder( compareTo );
    }

    private static int GetOrder( Breakpoint breakpoint )
    {
        return breakpoint.Normalize() switch
        {
            Breakpoint.None => 0,
            Breakpoint.Mobile => 1,
            Breakpoint.Tablet => 2,
            Breakpoint.Desktop => 3,
            Breakpoint.Widescreen => 4,
            Breakpoint.FullHD => 5,
            Breakpoint.QuadHD => 6,
            _ => 0,
        };
    }
}